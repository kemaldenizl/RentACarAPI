namespace Security.Domain.Auditing;

public enum AuditActionType
{
    UserRegistered = 1,
    LoginSucceeded = 2,
    LoginFailed = 3,
    RefreshSucceeded = 4,
    RefreshFailed = 5,
    RefreshReuseDetected = 6,
    LogoutCurrentSession = 7,
    LogoutAllSessions = 8,
    EmailVerificationRequested = 9,
    EmailVerified = 10,
    PasswordResetRequested = 11,
    PasswordResetCompleted = 12,
    RoleAssigned = 13,
    RoleRemoved = 14,
    PermissionAssignedToRole = 15,
    PermissionRemovedFromRole = 16
}