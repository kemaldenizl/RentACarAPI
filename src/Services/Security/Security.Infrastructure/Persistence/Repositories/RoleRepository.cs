using Microsoft.EntityFrameworkCore;
using Security.Application.Abstractions.Persistence;
using Security.Domain.Authorization;

namespace Security.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(SecurityDbContext dbContext) : IRoleRepository
{
    public async Task<Role?> GetByNormalizedNameAsync(
        string normalizedName,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.NormalizedName == normalizedName, cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetPermissionCodesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var permissionCodes = await (
            from userRole in dbContext.UserRoles.AsNoTracking()
            join rolePermission in dbContext.RolePermissions.AsNoTracking()
                on userRole.RoleId equals rolePermission.RoleId
            join permission in dbContext.Permissions.AsNoTracking()
                on rolePermission.PermissionId equals permission.Id
            where userRole.UserId == userId
            select permission.Code
        )
        .Distinct()
        .ToListAsync(cancellationToken);

        return permissionCodes;
    }
}