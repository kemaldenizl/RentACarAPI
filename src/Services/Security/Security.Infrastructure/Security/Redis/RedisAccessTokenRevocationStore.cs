using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Security.Application.Abstractions.Security;

namespace Security.Infrastructure.Security.Redis;

public sealed class RedisAccessTokenRevocationStore(
    IDistributedCache distributedCache,
    IOptions<RedisRevocationOptions> options
)   : IAccessTokenRevocationStore
{
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly RedisRevocationOptions _options = options.Value;

    public async Task RevokeAsync(
        string jti,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jti);

        var ttl = expiresAtUtc - DateTime.UtcNow;
        if (ttl <= TimeSpan.Zero)
        {
            ttl = TimeSpan.FromMinutes(1);
        }

        var key = BuildKey(jti);

        await _distributedCache.SetStringAsync(
            key,
            "1",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            },
            cancellationToken);
    }

    public async Task<bool> IsRevokedAsync(
        string jti,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jti);

        var key = BuildKey(jti);
        var value = await _distributedCache.GetStringAsync(key, cancellationToken);

        return !string.IsNullOrWhiteSpace(value);
    }

    private string BuildKey(string jti)
    {
        return $"{_options.InstanceName}:{_options.AccessTokenRevocationPrefix}{jti}";
    }
}