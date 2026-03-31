namespace Security.Application.Auth.Dtos;

public sealed record AccessTokenDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc
);