namespace Security.IntegrationTests.Contracts.Auth;

public sealed record CurrentUserResponse(
    Guid UserId,
    string Email,
    Guid? SessionId,
    string? AccessTokenJti,
    IReadOnlyCollection<string> Permissions
);