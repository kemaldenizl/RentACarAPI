namespace Security.API.Contracts.Auth;

public sealed record RefreshTokenRequest(
    string RefreshToken
);