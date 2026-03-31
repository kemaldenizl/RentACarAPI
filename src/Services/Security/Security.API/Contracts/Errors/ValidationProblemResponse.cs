namespace Security.API.Contracts.Errors;

public sealed record ValidationProblemResponse(
    string Title,
    int Status,
    string Detail,
    IDictionary<string, string[]> Errors
);