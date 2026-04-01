namespace Security.Application.Abstractions.Security;

public interface IAccessTokenRevocationStore
{
    Task RevokeAsync(
        string jti,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsRevokedAsync(
        string jti,
        CancellationToken cancellationToken = default
    );
}