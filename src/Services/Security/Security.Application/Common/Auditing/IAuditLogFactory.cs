using Security.Domain.Auditing;

namespace Security.Application.Abstractions.Auditing;

public interface IAuditLogFactory
{
    AuditLog Create(
        AuditActionType actionType,
        string payloadJson,
        Guid? userId = null
    );
}