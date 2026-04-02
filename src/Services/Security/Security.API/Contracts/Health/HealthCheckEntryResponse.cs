namespace Security.API.Contracts.Health;

public sealed record HealthCheckEntryResponse(
    string Status,
    string? Description,
    TimeSpan Duration,
    IReadOnlyDictionary<string, object?> Data
);