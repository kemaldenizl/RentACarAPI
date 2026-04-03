namespace Security.IntegrationTests.Contracts.Health;

public sealed record HealthCheckResponse(
    string Status,
    TimeSpan TotalDuration,
    IReadOnlyDictionary<string, HealthCheckEntryResponse> Entries
);