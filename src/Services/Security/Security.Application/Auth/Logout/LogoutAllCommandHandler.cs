using MediatR;
using Security.Application.Abstractions.Persistence;
using Security.Application.Abstractions.Time;
using Security.Application.Abstractions.UnitOfWork;
using Security.Application.Common.Results;
using Security.Domain.Auditing;

namespace Security.Application.Auth.Logout;

public sealed class LogoutAllCommandHandler(
    IRefreshSessionRepository refreshSessionRepository,
    IAuditLogRepository auditLogRepository,
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

        var revokedCount = sessions.Count;

        var auditLog = new AuditLog(
            Guid.NewGuid(),
            request.UserId,
            AuditActionType.LogoutAllSessions,
            request.IpAddress.Trim(),
            request.DeviceName.Trim(),
            Guid.NewGuid().ToString("N"),
            $$"""{"event":"logout_all_sessions","revokedCount":{{revokedCount}},"accessTokenJti":"{{request.AccessTokenJti}}"}""",
            utcNow);

        await auditLogRepository.AddAsync(auditLog, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}