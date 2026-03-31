using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Users;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(SecurityDbContext dbContext) : IUserRepository
{
    public async Task<bool> ExistsByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        return dbContext.Users.AddAsync(user, cancellationToken).AsTask();
    }
}