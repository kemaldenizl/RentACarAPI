using Security.Domain.Abstractions;
using Security.Domain.Common;

namespace Security.Domain.Users;

public sealed class User : AggregateRoot
{
    private readonly List<UserRole> _roles = [];

    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string NormalizedEmail { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public bool EmailVerified { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? LastLoginAtUtc { get; private set; }

    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    private User()
    {
    }

    public User(
        Guid id,
        string email,
        string normalizedEmail,
        string passwordHash,
        DateTime createdAtUtc)
    {
        Id = Guard.AgainstEmpty(id, nameof(id));
        Email = Guard.AgainstNullOrWhiteSpace(email, nameof(email));
        NormalizedEmail = Guard.AgainstNullOrWhiteSpace(normalizedEmail, nameof(normalizedEmail));
        PasswordHash = Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(passwordHash));
        CreatedAtUtc = Guard.AgainstDefault(createdAtUtc, nameof(createdAtUtc));

        EmailVerified = false;
        IsActive = true;
    }

    public void MarkEmailVerified()
    {
        EmailVerified = true;
    }

    public void MarkLogin(DateTime utcNow)
    {
        LastLoginAtUtc = Guard.AgainstDefault(utcNow, nameof(utcNow));
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        PasswordHash = Guard.AgainstNullOrWhiteSpace(newPasswordHash, nameof(newPasswordHash));
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void AddRole(Guid roleId, DateTime assignedAtUtc)
    {
        roleId = Guard.AgainstEmpty(roleId, nameof(roleId));
        assignedAtUtc = Guard.AgainstDefault(assignedAtUtc, nameof(assignedAtUtc));

        if (_roles.Any(x => x.RoleId == roleId))
            return;

        _roles.Add(new UserRole(Id, roleId, assignedAtUtc));
    }

    public void RemoveRole(Guid roleId)
    {
        roleId = Guard.AgainstEmpty(roleId, nameof(roleId));

        var existing = _roles.FirstOrDefault(x => x.RoleId == roleId);
        if (existing is null)
            return;

        _roles.Remove(existing);
    }
}