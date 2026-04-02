using MediatR;
using Security.Application.Common.Results;

namespace Security.Application.Auth.Logout;

public sealed record LogoutCommand(
    Guid UserId,
    Guid SessionId,
    string AccessTokenJti,
    DateTime AccessTokenExpiresAtUtc
) : IRequest<Result>;