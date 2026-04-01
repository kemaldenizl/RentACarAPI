namespace Security.API.Contracts.Sessions;

public sealed record SessionResponse(
    Guid SessionId,
    string DeviceName,
    string IpAddress,
    DateTime CreatedAtUtc,
    bool Revoked,
    DateTime? RevokedAtUtc,
    bool IsCurrent
);