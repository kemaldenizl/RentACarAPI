using MediatR;
using Security.Application.Abstractions.Auditing;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Security;
using Security.Application.Abstractions.Time;
using Security.Application.Abstractions.UnitOfWork;
using Security.Application.Common.Auditing;
using Security.Application.Common.Errors;
using Security.Application.Common.Results;
using Security.Domain.Auditing;

namespace Security.Application.Sessions.RevokeSession;

public sealed class RevokeSessionCommandHandler(
    IRefreshSessionRepository refreshSessionRepository,
    IAuditLogRepository auditLogRepository,
    IAuditLogFactory auditLogFactory,
    IAccessTokenRevocationStore accessTokenRevocationStore,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RevokeSessionCommand, Result>
{
    public async Task<Result> Handle(
        RevokeSessionCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = dateTimeProvider.UtcNow;

        var session = await refreshSessionRepository.GetByIdAsync(
            request.SessionId,
            cancellationToken);

        if (session is null)
        {
            return Result.Failure(AuthErrors.SessionNotFound);
        }

        if (session.UserId != request.UserId)
        {
            return Result.Failure(AuthErrors.InvalidSession);
        }

        session.Revoke(utcNow);

        var isCurrentSession = request.CurrentSessionId.HasValue &&
                               request.CurrentSessionId.Value == session.Id;

        if (isCurrentSession)
        {
            await accessTokenRevocationStore.RevokeAsync(
                request.AccessTokenJti,
                request.AccessTokenExpiresAtUtc,
                cancellationToken);
        }

        var auditLog = auditLogFactory.Create(
            AuditActionType.SessionRevoked,
            AuditPayloadBuilder.Build(new
            {
                @event = "session_revoked",
                sessionId = session.Id,
                isCurrentSession,
                accessTokenJti = request.AccessTokenJti
            }),
            request.UserId);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}