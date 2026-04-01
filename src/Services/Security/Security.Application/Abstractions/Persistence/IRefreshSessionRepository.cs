using Security.Domain.Sessions;

namespace Security.Application.Abstractions.Persistence;

public interface IRefreshSessionRepository
{
    Task AddAsync(RefreshSession session, CancellationToken cancellationToken = default);
    Task<RefreshSession?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default);
}