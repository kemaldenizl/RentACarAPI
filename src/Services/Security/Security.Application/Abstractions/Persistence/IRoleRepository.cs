using Security.Domain.Authorization;

namespace Security.Application.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<Role?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetPermissionCodesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}