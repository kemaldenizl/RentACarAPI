namespace Security.IntegrationTests.Contracts.Auth;

public sealed record RefreshTokenRequest(
    string RefreshToken
);