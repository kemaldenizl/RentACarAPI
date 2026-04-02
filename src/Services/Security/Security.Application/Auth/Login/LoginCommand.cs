using MediatR;
using Security.Application.Common.Results;

namespace Security.Application.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<LoginResponse>>;