namespace Security.Application.Sessions.Dtos;

public sealed record SessionDto(
    Guid SessionId,
    string DeviceName,
    string IpAddress,
    DateTime CreatedAtUtc,
    bool Revoked,
    DateTime? RevokedAtUtc,
    bool IsCurrent
);