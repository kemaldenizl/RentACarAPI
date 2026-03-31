using Security.Application.Abstractions.Persistence;
using Security.Domain.Sessions;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class RefreshSessionRepository(SecurityDbContext dbContext) : IRefreshSessionRepository
{
    public Task AddAsync(RefreshSession session, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);

        return dbContext.RefreshSessions.AddAsync(session, cancellationToken).AsTask();
    }
}