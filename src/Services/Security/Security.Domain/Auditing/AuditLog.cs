using Security.Domain.Abstractions;
using Security.Domain.Common;

namespace Security.Domain.Auditing;

public sealed class AuditLog : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public AuditActionType ActionType { get; private set; }
    public string IpAddress { get; private set; } = default!;
    public string UserAgent { get; private set; } = default!;
    public string CorrelationId { get; private set; } = default!;
    public string PayloadJson { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; }

    private AuditLog()
    {
    }

    public AuditLog(
        Guid id,
        Guid? userId,
        AuditActionType actionType,
        string ipAddress,
        string userAgent,
        string correlationId,
        string payloadJson,
        DateTime createdAtUtc)
    {
        Id = Guard.AgainstEmpty(id, nameof(id));
        UserId = userId;
        ActionType = actionType;
        IpAddress = Guard.AgainstNullOrWhiteSpace(ipAddress, nameof(ipAddress));
        UserAgent = Guard.AgainstNullOrWhiteSpace(userAgent, nameof(userAgent));
        CorrelationId = Guard.AgainstNullOrWhiteSpace(correlationId, nameof(correlationId));
        PayloadJson = Guard.AgainstNullOrWhiteSpace(payloadJson, nameof(payloadJson));
        CreatedAtUtc = Guard.AgainstDefault(createdAtUtc, nameof(createdAtUtc));
    }
}