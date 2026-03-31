namespace Security.Application.Common.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = new("auth.invalid_credentials", "Invalid credentials.");
    public static readonly Error UserAlreadyExists = new("auth.user_already_exists", "User already exists.");
    public static readonly Error UserInactive = new("auth.user_inactive", "User is inactive.");
    public static readonly Error EmailNotVerified = new("auth.email_not_verified", "Email is not verified.");
    public static readonly Error RoleNotFound = new("auth.role_not_found", "Required role was not found.");
}