namespace Security.API.Contracts.Auth;

public sealed record RefreshTokenResponse(
    AuthTokensResponse Tokens
);