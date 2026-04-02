namespace Security.IntegrationTests.Contracts.Common;

public sealed record ProblemDetailsResponse(
    string? Type,
    string? Title,
    int? Status,
    string? Detail,
    string? TraceId
);