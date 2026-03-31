using Security.Domain.Common;

namespace Security.Domain.Users;

public sealed class UserRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }

    private UserRole()
    {
    }

    public UserRole(Guid userId, Guid roleId, DateTime assignedAtUtc)
    {
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        RoleId = Guard.AgainstEmpty(roleId, nameof(roleId));
        AssignedAtUtc = Guard.AgainstDefault(assignedAtUtc, nameof(assignedAtUtc));
    }
}