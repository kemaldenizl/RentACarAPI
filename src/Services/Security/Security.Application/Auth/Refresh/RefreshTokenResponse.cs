using Security.Application.Auth.Dtos;

namespace Security.Application.Auth.Refresh;

public sealed record RefreshTokenResponse(AuthTokensDto Tokens);