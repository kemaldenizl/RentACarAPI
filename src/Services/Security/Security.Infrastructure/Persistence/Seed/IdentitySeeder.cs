using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security.Domain.Authorization;
using Security.Domain.Users;
using Security.Infrastructure.Security;
using Microsoft.Extensions.Hosting;

namespace Security.Infrastructure.Persistence.Seed;

public sealed class IdentitySeeder(
    SecurityDbContext dbContext,
    PasswordHasher passwordHasher,
    IOptions<IdentitySeedOptions> options,
    IHostEnvironment hostEnvironment,
    ILogger<IdentitySeeder> logger
)
{
    private readonly IdentitySeedOptions _options = options.Value;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            logger.LogInformation("Identity seed is disabled.");
            return;
        }

        if (_hostEnvironment.IsProduction() && !_options.AllowInProduction)
        {
            logger.LogWarning("Identity seed is blocked in Production environment.");
            return;
        }

        await SeedPermissionsAsync(cancellationToken);
        await SeedRolesAsync(cancellationToken);
        await SeedAdminUserAsync(cancellationToken);
    }

    private async Task SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        var permissionCodes = new[]
        {
            PermissionCodes.UsersRead,
            PermissionCodes.UsersManage,
            PermissionCodes.RolesRead,
            PermissionCodes.RolesManage,
            PermissionCodes.PermissionsRead,
            PermissionCodes.PermissionsManage,
            PermissionCodes.SessionsRead,
            PermissionCodes.SessionsManage
        };

        var existingCodes = await dbContext.Permissions
            .AsNoTracking()
            .Select(x => x.Code)
            .ToListAsync(cancellationToken);

        var missingPermissions = permissionCodes
            .Except(existingCodes, StringComparer.OrdinalIgnoreCase)
            .Select(code => new Permission(Guid.NewGuid(), code))
            .ToList();

        if (missingPermissions.Count == 0)
            return;

        await dbContext.Permissions.AddRangeAsync(missingPermissions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        const string adminRoleName = "Admin";
        const string adminRoleNormalizedName = "ADMIN";

        var adminRole = await dbContext.Roles
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.NormalizedName == adminRoleNormalizedName, cancellationToken);

        if (adminRole is null)
        {
            adminRole = new Role(Guid.NewGuid(), adminRoleName, adminRoleNormalizedName);
            await dbContext.Roles.AddAsync(adminRole, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var allPermissionIds = await dbContext.Permissions
            .AsNoTracking()
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var permissionId in allPermissionIds)
        {
            adminRole.AddPermission(permissionId, DateTime.UtcNow);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    {
        var normalizedEmail = _options.AdminEmail.Trim().ToUpperInvariant();

        var adminUser = await dbContext.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

        if (adminUser is null)
        {
            adminUser = new User(
                Guid.NewGuid(),
                _options.AdminEmail.Trim(),
                normalizedEmail,
                passwordHasher.Hash(_options.AdminPassword),
                DateTime.UtcNow);

            adminUser.MarkEmailVerified();
            await dbContext.Users.AddAsync(adminUser, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var adminRole = await dbContext.Roles
            .AsNoTracking()
            .FirstAsync(x => x.NormalizedName == "ADMIN", cancellationToken);

        adminUser.AddRole(adminRole.Id, DateTime.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}