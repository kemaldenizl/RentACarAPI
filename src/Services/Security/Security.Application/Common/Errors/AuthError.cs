namespace Security.Application.Common.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = new("auth.invalid_credentials", "Invalid credentials.");
    public static readonly Error UserAlreadyExists = new("auth.user_already_exists", "User already exists.");
    public static readonly Error UserInactive = new("auth.user_inactive", "User is inactive.");
    public static readonly Error EmailNotVerified = new("auth.email_not_verified", "Email is not verified.");
    public static readonly Error RoleNotFound = new("auth.role_not_found", "Required role was not found.");
    public static readonly Error InvalidRefreshToken = new("auth.invalid_refresh_token", "Refresh token is invalid.");
    public static readonly Error ExpiredRefreshToken = new("auth.expired_refresh_token", "Refresh token is expired.");
    public static readonly Error RevokedRefreshToken = new("auth.revoked_refresh_token", "Refresh token is revoked.");   
    public static readonly Error ConsumedRefreshToken = new("auth.consumed_refresh_token", "Refresh token has already been used.");
    public static readonly Error SessionRevoked = new("auth.session_revoked", "Session is revoked.");
    public static readonly Error RefreshTokenReuseDetected = new("auth.refresh_token_reuse_detected", "Refresh token reuse detected. Session has been revoked.");
}