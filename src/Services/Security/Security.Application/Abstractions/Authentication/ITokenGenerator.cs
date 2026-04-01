using Security.Application.Auth.Dtos;

namespace Security.Application.Abstractions.Authentication;

public interface ITokenGenerator
{
    Task<AccessTokenDto> GenerateAccessTokenAsync(
        Guid userId,
        string email,
        IReadOnlyCollection<string> permissions,
        Guid? sessionId = null,
        CancellationToken cancellationToken = default);
}