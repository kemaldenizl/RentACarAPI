namespace Security.API.Contracts.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password
);