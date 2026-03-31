using Security.Application.Auth.Dtos;

namespace Security.Application.Auth.Login;

public sealed record LoginResponse(
    UserDto User,
    AuthTokensDto Tokens
);