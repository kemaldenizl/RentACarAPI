using Security.Domain.Abstractions;
using Security.Domain.Common;

namespace Security.Domain.Sessions;

public sealed class RefreshSession : AggregateRoot
{
    private readonly List<RefreshToken> _tokens = [];

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string DeviceName { get; private set; } = default!;
    public string IpAddress { get; private set; } = default!;
    public bool Revoked { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    public IReadOnlyCollection<RefreshToken> Tokens => _tokens.AsReadOnly();

    private RefreshSession()
    {
    }

    public RefreshSession(
        Guid id,
        Guid userId,
        string deviceName,
        string ipAddress,
        DateTime createdAtUtc)
    {
        Id = Guard.AgainstEmpty(id, nameof(id));
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        DeviceName = Guard.AgainstNullOrWhiteSpace(deviceName, nameof(deviceName));
        IpAddress = Guard.AgainstNullOrWhiteSpace(ipAddress, nameof(ipAddress));
        CreatedAtUtc = Guard.AgainstDefault(createdAtUtc, nameof(createdAtUtc));
    }

    public void AddToken(RefreshToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        Guard.Against(Revoked, "Cannot add token to a revoked session.");

        _tokens.Add(token);
    }

    public void Revoke(DateTime utcNow)
    {
        utcNow = Guard.AgainstDefault(utcNow, nameof(utcNow));

        if (Revoked)
            return;

        Revoked = true;
        RevokedAtUtc = utcNow;

        foreach (var token in _tokens.Where(x => !x.Revoked))
        {
            token.Revoke();
        }
    }

    public RefreshToken? GetLatestActiveToken(DateTime utcNow)
    {
        utcNow = Guard.AgainstDefault(utcNow, nameof(utcNow));

        return _tokens
            .Where(x => x.IsUsable(utcNow))
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefault();
    }

    public bool ContainsTokenHash(string tokenHash)
    {
        tokenHash = Guard.AgainstNullOrWhiteSpace(tokenHash, nameof(tokenHash));
        return _tokens.Any(x => x.TokenHash == tokenHash);
    }

    public RefreshToken? GetTokenByHash(string tokenHash)
    {
        tokenHash = Guard.AgainstNullOrWhiteSpace(tokenHash, nameof(tokenHash));
        return _tokens.FirstOrDefault(x => x.TokenHash == tokenHash);
    }
}