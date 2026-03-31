namespace Security.API.Contracts.Auth;

public sealed record CurrentUserResponse(
    Guid UserId,
    string Email,
    IReadOnlyCollection<string> Permissions
);