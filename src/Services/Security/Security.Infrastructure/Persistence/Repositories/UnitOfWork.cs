using Security.Application.Abstractions.UnitOfWork;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork(SecurityDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}