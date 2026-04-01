namespace Security.Infrastructure.Security.Redis;

public sealed class RedisRevocationOptions
{
    public const string SectionName = "RedisRevocation";
    public string InstanceName { get; init; } = "security";
    public string AccessTokenRevocationPrefix { get; init; } = "revoked:access:jti:";
}