namespace Security.IntegrationTests.Contracts.Auth;

public sealed record RefreshTokenResponse(
    AuthTokensResponse Tokens
);