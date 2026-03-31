using Security.Domain.Common;

namespace Security.Domain.Authorization;

public sealed class RolePermission
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }

    private RolePermission()
    {
    }

    public RolePermission(Guid roleId, Guid permissionId, DateTime assignedAtUtc)
    {
        RoleId = Guard.AgainstEmpty(roleId, nameof(roleId));
        PermissionId = Guard.AgainstEmpty(permissionId, nameof(permissionId));
        AssignedAtUtc = Guard.AgainstDefault(assignedAtUtc, nameof(assignedAtUtc));
    }
}