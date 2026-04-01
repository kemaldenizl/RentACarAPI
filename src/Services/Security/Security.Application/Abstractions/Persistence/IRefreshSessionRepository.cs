using Security.Domain.Sessions;

namespace Security.Application.Abstractions.Persistence;

public interface IRefreshSessionRepository
{
    Task AddAsync(RefreshSession session, CancellationToken cancellationToken = default);
    Task<RefreshSession?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task<RefreshSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RefreshSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}