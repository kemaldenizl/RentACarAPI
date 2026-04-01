namespace Security.API.Common.Auth;

public sealed record CurrentUser(
    Guid UserId,
    string Email,
    Guid? SessionId,
    string? AccessTokenJti,
    IReadOnlyCollection<string> Permissions
);