namespace Security.API.Contracts.Auth;

public sealed record UserResponse(
    Guid Id,
    string Email,
    bool EmailVerified,
    bool IsActive
);