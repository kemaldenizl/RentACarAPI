namespace Security.IntegrationTests.Contracts.Auth;

public sealed record LoginResponse(
    UserResponse User,
    AuthTokensResponse Tokens
);