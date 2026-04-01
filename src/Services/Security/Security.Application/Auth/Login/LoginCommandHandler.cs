using MediatR;
using Security.Application.Abstractions.Authentication;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Application.Abstractions.Time;
using Security.Application.Abstractions.UnitOfWork;
using Security.Application.Auth.Dtos;
using Security.Application.Common.Errors;
using Security.Application.Common.Results;
using Security.Domain.Auditing;
using Security.Domain.Sessions;

namespace Security.Application.Auth.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRefreshSessionRepository refreshSessionRepository,
    IAuditLogRepository auditLogRepository,
    IPasswordHasher passwordHasher,
    IRefreshTokenGenerator refreshTokenGenerator,
    ITokenGenerator tokenGenerator,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();

        var user = await userRepository.GetByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            await WriteFailedLoginAuditAsync(normalizedEmail, request.IpAddress, cancellationToken);
            return Result<LoginResponse>.Failure(AuthErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            await WriteFailedLoginAuditAsync(normalizedEmail, request.IpAddress, cancellationToken);
            return Result<LoginResponse>.Failure(AuthErrors.UserInactive);
        }

        var passwordValid = passwordHasher.Verify(user.PasswordHash, request.Password);
        if (!passwordValid)
        {
            await WriteFailedLoginAuditAsync(normalizedEmail, request.IpAddress, cancellationToken);
            return Result<LoginResponse>.Failure(AuthErrors.InvalidCredentials);
        }

        var utcNow = dateTimeProvider.UtcNow;
        user.MarkLogin(utcNow);

        var refreshTokenPair = refreshTokenGenerator.Generate();
        var refreshTokenExpiresAtUtc = utcNow.AddDays(30);

        var session = new RefreshSession(
            Guid.NewGuid(),
            user.Id,
            request.DeviceName.Trim(),
            request.IpAddress.Trim(),
            utcNow);

        var refreshToken = new RefreshToken(
            Guid.NewGuid(),
            session.Id,
            refreshTokenPair.HashedToken,
            refreshTokenExpiresAtUtc,
            utcNow);

        session.AddToken(refreshToken);

        var permissions = await roleRepository.GetPermissionCodesByUserIdAsync(user.Id, cancellationToken);

        var accessToken = await tokenGenerator.GenerateAccessTokenAsync(
            user.Id,
            user.Email,
            permissions,
            session.Id,
            cancellationToken);

        await refreshSessionRepository.AddAsync(session, cancellationToken);

        var successAudit = new AuditLog(
            Guid.NewGuid(),
            user.Id,
            AuditActionType.LoginSucceeded,
            request.IpAddress.Trim(),
            request.DeviceName.Trim(),
            Guid.NewGuid().ToString("N"),
            $$"""{"event":"login_succeeded","email":"{{user.Email}}","sessionId":"{{session.Id}}"}""",
            utcNow);

        await auditLogRepository.AddAsync(successAudit, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse(
            new UserDto(user.Id, user.Email, user.EmailVerified, user.IsActive),
            new AuthTokensDto(
                accessToken.AccessToken,
                accessToken.AccessTokenExpiresAtUtc,
                refreshTokenPair.PlainTextToken,
                refreshTokenExpiresAtUtc));

        return Result<LoginResponse>.Success(response);
    }

    private async Task WriteFailedLoginAuditAsync(
        string normalizedEmail,
        string ipAddress,
        CancellationToken cancellationToken)
    {
        var auditLog = new AuditLog(
            Guid.NewGuid(),
            null,
            AuditActionType.LoginFailed,
            ipAddress.Trim(),
            "unknown",
            Guid.NewGuid().ToString("N"),
            $$"""{"event":"login_failed","normalizedEmail":"{{normalizedEmail}}"}""",
            dateTimeProvider.UtcNow);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}