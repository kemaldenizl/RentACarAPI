using MediatR;
using Security.Application.Common.Results;

namespace Security.Application.Sessions.RevokeSession;

public sealed record RevokeSessionCommand(
    Guid UserId,
    Guid SessionId,
    string AccessTokenJti,
    DateTime AccessTokenExpiresAtUtc,
    Guid? CurrentSessionId,
    string DeviceName,
    string IpAddress) : IRequest<Result>;