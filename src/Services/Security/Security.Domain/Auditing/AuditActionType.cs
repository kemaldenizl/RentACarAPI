namespace Security.Domain.Auditing;

public enum AuditActionType
{
    UserRegistered = 1,
    LoginSucceeded = 2,
    LoginFailed = 3,
    RefreshSucceeded = 4,
    RefreshReuseDetected = 5,
    LogoutCurrentSession = 6,
    LogoutAllSessions = 7,
    EmailVerificationRequested = 8,
    EmailVerified = 9,
    PasswordResetRequested = 10,
    PasswordResetCompleted = 11,
    RoleAssigned = 12,
    RoleRemoved = 13,
    PermissionAssignedToRole = 14,
    PermissionRemovedFromRole = 15
}