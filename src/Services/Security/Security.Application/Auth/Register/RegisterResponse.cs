using Security.Application.Auth.Dtos;

namespace Security.Application.Auth.Register;

public sealed record RegisterResponse(UserDto User);