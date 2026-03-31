namespace Security.Domain.Authorization;

public static class PermissionCodes
{
    public const string UsersRead = "users.read";
    public const string UsersManage = "users.manage";

    public const string RolesRead = "roles.read";
    public const string RolesManage = "roles.manage";

    public const string PermissionsRead = "permissions.read";
    public const string PermissionsManage = "permissions.manage";

    public const string SessionsRead = "sessions.read";
    public const string SessionsManage = "sessions.manage";
}