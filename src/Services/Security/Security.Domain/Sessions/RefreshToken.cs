using Security.Domain.Common;

namespace Security.Domain.Sessions;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool Consumed { get; private set; }
    public DateTime? ConsumedAtUtc { get; private set; }
    public bool Revoked { get; private set; }

    private RefreshToken()
    {
    }

    public RefreshToken(
        Guid id,
        Guid sessionId,
        string tokenHash,
        DateTime expiresAtUtc,
        DateTime createdAtUtc)
    {
        Id = Guard.AgainstEmpty(id, nameof(id));
        SessionId = Guard.AgainstEmpty(sessionId, nameof(sessionId));
        TokenHash = Guard.AgainstNullOrWhiteSpace(tokenHash, nameof(tokenHash));
        ExpiresAtUtc = Guard.AgainstDefault(expiresAtUtc, nameof(expiresAtUtc));
        CreatedAtUtc = Guard.AgainstDefault(createdAtUtc, nameof(createdAtUtc));

        Guard.Against(expiresAtUtc <= createdAtUtc, "Refresh token expiry must be after creation time.");
    }

    public void Consume(DateTime utcNow)
    {
        utcNow = Guard.AgainstDefault(utcNow, nameof(utcNow));

        Guard.Against(Revoked, "Revoked refresh token cannot be consumed.");
        Guard.Against(Consumed, "Refresh token is already consumed.");

        Consumed = true;
        ConsumedAtUtc = utcNow;
    }

    public void Revoke()
    {
        Revoked = true;
    }

    public bool IsExpired(DateTime utcNow)
    {
        utcNow = Guard.AgainstDefault(utcNow, nameof(utcNow));
        return utcNow >= ExpiresAtUtc;
    }

    public bool IsUsable(DateTime utcNow)
    {
        return !Revoked && !Consumed && !IsExpired(utcNow);
    }
}