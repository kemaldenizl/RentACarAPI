using MediatR;
using Security.Application.Abstractions.Auditing;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Application.Abstractions.Time;
using Security.Application.Abstractions.UnitOfWork;
using Security.Application.Common.Auditing;
using Security.Application.Common.Results;
using Security.Domain.Auditing;

namespace Security.Application.Auth.Logout;

public sealed class LogoutAllCommandHandler(
    IRefreshSessionRepository refreshSessionRepository,
    IAuditLogRepository auditLogRepository,
    IAuditLogFactory auditLogFactory,
    IAccessTokenRevocationStore accessTokenRevocationStore,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LogoutAllCommand, Result>
{
    public async Task<Result> Handle(
        LogoutAllCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = dateTimeProvider.UtcNow;

        var sessions = await refreshSessionRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke(utcNow);
        }

        await accessTokenRevocationStore.RevokeAsync(
            request.AccessTokenJti,
            request.AccessTokenExpiresAtUtc,
            cancellationToken);

        var auditLog = auditLogFactory.Create(
            AuditActionType.LogoutAllSessions,
            AuditPayloadBuilder.Build(new
            {
                @event = "logout_all_sessions",
                revokedCount = sessions.Count,
                accessTokenJti = request.AccessTokenJti
            }),
            request.UserId);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}