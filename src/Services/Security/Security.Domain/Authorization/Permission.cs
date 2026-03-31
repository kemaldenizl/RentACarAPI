using Security.Domain.Common;

namespace Security.Domain.Authorization;

public sealed class Permission
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = default!;

    private Permission()
    {
    }

    public Permission(Guid id, string code)
    {
        Id = Guard.AgainstEmpty(id, nameof(id));
        Code = Guard.AgainstNullOrWhiteSpace(code, nameof(code));
    }

    public void Rename(string code)
    {
        Code = Guard.AgainstNullOrWhiteSpace(code, nameof(code));
    }
}