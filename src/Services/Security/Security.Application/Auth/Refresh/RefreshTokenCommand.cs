using MediatR;
using Security.Application.Common.Results;

namespace Security.Application.Auth.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;