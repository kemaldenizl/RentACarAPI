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
            await WriteRefreshFailedAuditAsync(
                null,
                request.IpAddress,
                request.DeviceName,
                "not_found",
                utcNow,
                cancellationToken);

            return Result<RefreshTokenResponse>.Failure(AuthErrors.InvalidRefreshToken);
        }

        var existingToken = session.GetTokenByHash(hashedRefreshToken);
        if (existingToken is null)
        {
            await RevokeSessionForReuseSuspicionAsync(
                session,
                request.IpAddress,
                request.DeviceName,
                "token_not_attached_to_session",
                utcNow,
                cancellationToken);

            return Result<RefreshTokenResponse>.Failure(AuthErrors.RefreshTokenReuseDetected);
        }

        if (session.Revoked)
        {
            await WriteRefreshFailedAuditAsync(
                session.UserId,
                request.IpAddress,
                request.DeviceName,
                "session_revoked",
                utcNow,
                cancellationToken);

            return Result<RefreshTokenResponse>.Failure(AuthErrors.SessionRevoked);
        }

        if (existingToken.Revoked || existingToken.Consumed)
        {
            await RevokeSessionForReuseSuspicionAsync(
                session,
                request.IpAddress,
                request.DeviceName,
                existingToken.Revoked ? "revoked_token_reused" : "consumed_token_reused",
                utcNow,
                cancellationToken);

            return Result<RefreshTokenResponse>.Failure(AuthErrors.RefreshTokenReuseDetected);
        }

        if (existingToken.IsExpired(utcNow))
        {
            await WriteRefreshFailedAuditAsync(
                session.UserId,
                request.IpAddress,
                request.DeviceName,
                "token_expired",
                utcNow,
                cancellationToken);

            return Result<RefreshTokenResponse>.Failure(AuthErrors.ExpiredRefreshToken);
        }

        var user = await userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            await WriteRefreshFailedAuditAsync(
                session.UserId,
                request.IpAddress,
                request.DeviceName,
                "user_not_active",
                utcNow,
                cancellationToken);

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

    private async Task RevokeSessionForReuseSuspicionAsync(
        RefreshSession session,
        string ipAddress,
        string deviceName,
        string reason,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        session.Revoke(utcNow);

        var auditLog = new AuditLog(
            Guid.NewGuid(),
            session.UserId,
            AuditActionType.RefreshReuseDetected,
            ipAddress.Trim(),
            deviceName.Trim(),
            Guid.NewGuid().ToString("N"),
            $$"""{"event":"refresh_reuse_detected","reason":"{{reason}}","sessionId":"{{session.Id}}"}""",
            utcNow);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task WriteRefreshFailedAuditAsync(
        Guid? userId,
        string ipAddress,
        string deviceName,
        string reason,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        var auditLog = new AuditLog(
            Guid.NewGuid(),
            userId,
            AuditActionType.RefreshFailed,
            ipAddress.Trim(),
            deviceName.Trim(),
            Guid.NewGuid().ToString("N"),
            $$"""{"event":"refresh_failed","reason":"{{reason}}"}""",
            utcNow);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}