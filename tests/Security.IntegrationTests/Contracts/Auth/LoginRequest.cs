namespace Security.IntegrationTests.Contracts.Auth;

public sealed record LoginRequest(
    string Email,
    string Password
);