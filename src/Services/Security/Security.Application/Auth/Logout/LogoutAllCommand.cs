using MediatR;
using Security.Application.Common.Results;

namespace Security.Application.Auth.Logout;

public sealed record LogoutAllCommand(
    Guid UserId,
    string AccessTokenJti,
    DateTime AccessTokenExpiresAtUtc,
    string DeviceName,
    string IpAddress
) : IRequest<Result>;