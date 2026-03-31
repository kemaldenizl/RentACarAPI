using MediatR;
using Security.Application.Common.Results;

namespace Security.Application.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password
) : IRequest<Result<RegisterResponse>>;