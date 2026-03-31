namespace Security.API.Common.Auth;

public sealed record CurrentUser(
    Guid UserId,
    string Email,
    IReadOnlyCollection<string> Permissions
);