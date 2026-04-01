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

namespace Security.Application.Auth.Refresh;

public sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRefreshSessionRepository refreshSessionRepository,
    IAuditLogRepository auditLogRepository,
    IRefreshTokenGenerator refreshTokenGenerator,
    ITokenGenerator tokenGenerator,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = dateTimeProvider.UtcNow;
        var hashedRefreshToken = refreshTokenGenerator.Hash(request.RefreshToken);

        var session = await refreshSessionRepository.GetByRefreshTokenHashAsync(
            hashedRefreshToken,
            cancellationToken);

        if (session is null)
        {
            await WriteFailedRefreshAuditAsync(null, request.IpAddress, "not_found", utcNow, cancellationToken);
            return Result<RefreshTokenResponse>.Failure(AuthErrors.InvalidRefreshToken);
        }

        if (session.Revoked)
        {
            await WriteFailedRefreshAuditAsync(session.UserId, request.IpAddress, "session_revoked", utcNow, cancellationToken);
            return Result<RefreshTokenResponse>.Failure(AuthErrors.SessionRevoked);
        }

        var existingToken = session.Tokens.FirstOrDefault(x => x.TokenHash == hashedRefreshToken);
        if (existingToken is null)
        {
            await WriteFailedRefreshAuditAsync(session.UserId, request.IpAddress, "token_not_attached_to_session", utcNow, cancellationToken);
            return Result<RefreshTokenResponse>.Failure(AuthErrors.InvalidRefreshToken);
        }

        if (existingToken.Revoked)
        {
            await WriteFailedRefreshAuditAsync(session.UserId, request.IpAddress, "token_revoked", utcNow, cancellationToken);
            return Result<RefreshTokenResponse>.Failure(AuthErrors.RevokedRefreshToken);
        }

        if (existingToken.Consumed)
        {
            await WriteFailedRefreshAuditAsync(session.UserId, request.IpAddress, "token_consumed", utcNow, cancellationToken);
            return Result<RefreshTokenResponse>.Failure(AuthErrors.ConsumedRefreshToken);
        }

        if (existingToken.IsExpired(utcNow))
        {
            await WriteFailedRefreshAuditAsync(session.UserId, request.IpAddress, "token_expired", utcNow, cancellationToken);
            return Result<RefreshTokenResponse>.Failure(AuthErrors.ExpiredRefreshToken);
        }

        var user = await userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            await WriteFailedRefreshAuditAsync(session.UserId, request.IpAddress, "user_not_active", utcNow, cancellationToken);
            return Result<RefreshTokenResponse>.Failure(AuthErrors.InvalidRefreshToken);
        }

        existingToken.Consume(utcNow);

        var permissions = await roleRepository.GetPermissionCodesByUserIdAsync(user.Id, cancellationToken);

        var accessToken = await tokenGenerator.GenerateAccessTokenAsync(
            user.Id,
            user.Email,
            permissions,
            cancellationToken);

        var newRefreshTokenPair = refreshTokenGenerator.Generate();
        var newRefreshTokenExpiresAtUtc = utcNow.Add(RefreshTokenLifetime);

        var rotatedRefreshToken = new RefreshToken(
            Guid.NewGuid(),
            session.Id,
            newRefreshTokenPair.HashedToken,
            newRefreshTokenExpiresAtUtc,
            utcNow);

        session.AddToken(rotatedRefreshToken);

        var auditLog = new AuditLog(
            Guid.NewGuid(),
            user.Id,
            AuditActionType.RefreshSucceeded,
            request.IpAddress.Trim(),
            request.DeviceName.Trim(),
            Guid.NewGuid().ToString("N"),
            $$"""{"event":"refresh_succeeded","sessionId":"{{session.Id}}"}""",
            utcNow);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RefreshTokenResponse(
            new AuthTokensDto(
                accessToken.AccessToken,
                accessToken.AccessTokenExpiresAtUtc,
                newRefreshTokenPair.PlainTextToken,
                newRefreshTokenExpiresAtUtc));

        return Result<RefreshTokenResponse>.Success(response);
    }

    private async Task WriteFailedRefreshAuditAsync(
        Guid? userId,
        string ipAddress,
        string reason,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        var auditLog = new AuditLog(
            Guid.NewGuid(),
            userId,
            AuditActionType.RefreshFailed,
            ipAddress.Trim(),
            "unknown",
            Guid.NewGuid().ToString("N"),
            $$"""{"event":"refresh_failed","reason":"{{reason}}"}""",
            utcNow);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}