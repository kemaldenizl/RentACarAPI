namespace Security.API.Contracts.Auth;

public sealed record LoginResponse(
    UserResponse User,
    AuthTokensResponse Tokens
);