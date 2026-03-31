using Security.Application.Abstractions.Persistence;
using Security.Domain.Auditing;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository(SecurityDbContext dbContext) : IAuditLogRepository
{
    public Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(auditLog);

        return dbContext.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();
    }
}