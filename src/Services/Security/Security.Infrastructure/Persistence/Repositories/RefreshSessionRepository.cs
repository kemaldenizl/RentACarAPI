using Microsoft.EntityFrameworkCore;
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

    public async Task<RefreshSession?> GetByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshSessions
            .Include(x => x.Tokens)
            .FirstOrDefaultAsync(
                x => x.Tokens.Any(t => t.TokenHash == refreshTokenHash),
                cancellationToken);
    }

    public async Task<RefreshSession?> GetByIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshSessions
            .Include(x => x.Tokens)
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<RefreshSession>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshSessions
            .Include(x => x.Tokens)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}