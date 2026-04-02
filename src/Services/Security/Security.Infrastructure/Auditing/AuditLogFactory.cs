using Security.Application.Abstractions.Auditing;
using Security.Application.Abstractions.RequestContext;
using Security.Application.Abstractions.Time;
using Security.Domain.Auditing;

namespace Security.Infrastructure.Auditing;

public sealed class AuditLogFactory(
    IRequestContext requestContext,
    IDateTimeProvider dateTimeProvider
) : IAuditLogFactory
{
    public AuditLog Create(
        AuditActionType actionType,
        string payloadJson,
        Guid? userId = null)
    {
        return new AuditLog(
            Guid.NewGuid(),
            userId,
            actionType,
            requestContext.IpAddress,
            requestContext.UserAgent,
            requestContext.CorrelationId,
            payloadJson,
            dateTimeProvider.UtcNow);
    }
}