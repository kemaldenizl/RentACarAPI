using Security.Domain.Abstractions;
using Security.Domain.Common;

namespace Security.Domain.Authorization;

public sealed class Role : AggregateRoot
{
    private readonly List<RolePermission> _permissions = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string NormalizedName { get; private set; } = default!;

    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role()
    {
    }

    public Role(Guid id, string name, string normalizedName)
    {
        Id = Guard.AgainstEmpty(id, nameof(id));
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        NormalizedName = Guard.AgainstNullOrWhiteSpace(normalizedName, nameof(normalizedName));
    }

    public void Rename(string name, string normalizedName)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        NormalizedName = Guard.AgainstNullOrWhiteSpace(normalizedName, nameof(normalizedName));
    }

    public void AddPermission(Guid permissionId, DateTime assignedAtUtc)
    {
        permissionId = Guard.AgainstEmpty(permissionId, nameof(permissionId));
        assignedAtUtc = Guard.AgainstDefault(assignedAtUtc, nameof(assignedAtUtc));

        if (_permissions.Any(x => x.PermissionId == permissionId))
            return;

        _permissions.Add(new RolePermission(Id, permissionId, assignedAtUtc));
    }

    public void RemovePermission(Guid permissionId)
    {
        permissionId = Guard.AgainstEmpty(permissionId, nameof(permissionId));

        var existing = _permissions.FirstOrDefault(x => x.PermissionId == permissionId);
        if (existing is null)
            return;

        _permissions.Remove(existing);
    }
}