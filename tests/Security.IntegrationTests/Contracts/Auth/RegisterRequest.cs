namespace Security.IntegrationTests.Contracts.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password
);