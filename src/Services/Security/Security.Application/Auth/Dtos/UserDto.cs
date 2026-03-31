namespace Security.Application.Auth.Dtos;

public sealed record UserDto(
    Guid Id,
    string Email,
    bool EmailVerified,
    bool IsActive
);