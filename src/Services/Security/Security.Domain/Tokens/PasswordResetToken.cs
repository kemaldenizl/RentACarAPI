using Security.Domain.Common;

namespace Security.Domain.Tokens;

public sealed class PasswordResetToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool Used { get; private set; }
    public DateTime? UsedAtUtc { get; private set; }

    private PasswordResetToken()
    {
    }

    public PasswordResetToken(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc,
        DateTime createdAtUtc)
    {
        Id = Guard.AgainstEmpty(id, nameof(id));
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        TokenHash = Guard.AgainstNullOrWhiteSpace(tokenHash, nameof(tokenHash));
        ExpiresAtUtc = Guard.AgainstDefault(expiresAtUtc, nameof(expiresAtUtc));
        CreatedAtUtc = Guard.AgainstDefault(createdAtUtc, nameof(createdAtUtc));

        Guard.Against(expiresAtUtc <= createdAtUtc, "Password reset token expiry must be after creation time.");
    }

    public void MarkUsed(DateTime utcNow)
    {
        utcNow = Guard.AgainstDefault(utcNow, nameof(utcNow));

        Guard.Against(Used, "Password reset token is already used.");

        Used = true;
        UsedAtUtc = utcNow;
    }

    public bool IsExpired(DateTime utcNow)
    {
        utcNow = Guard.AgainstDefault(utcNow, nameof(utcNow));
        return utcNow >= ExpiresAtUtc;
    }

    public bool IsUsable(DateTime utcNow)
    {
        return !Used && !IsExpired(utcNow);
    }
}