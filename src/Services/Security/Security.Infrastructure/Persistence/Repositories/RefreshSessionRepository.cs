using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Sessions;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class RefreshSessionRepository : IRefreshSessionRepository
{
    private readonly SecurityDbContext _dbContext;

    public RefreshSessionRepository(SecurityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(RefreshSession session, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(session);

        return _dbContext.RefreshSessions.AddAsync(session, cancellationToken).AsTask();
    }

    public async Task<RefreshSession?> GetByRefreshTokenHashAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshTokenHash);

        Guid sessionId = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(x => x.TokenHash == refreshTokenHash)
            .Select(x => x.SessionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (sessionId == Guid.Empty)
        {
            return null;
        }

        return await _dbContext.RefreshSessions
            .Include(x => x.Tokens)
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
    }
}