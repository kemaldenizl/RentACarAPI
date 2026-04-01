namespace Security.API.Common.Auth;

public sealed record CurrentAccessToken(
    string Jti,
    DateTime ExpiresAtUtc
);