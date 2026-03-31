using Security.Application.Auth.Dtos;

namespace Security.Application.Abstractions.Authentication;

public interface ITokenGenerator
{
    Task<AuthTokensDto> GenerateAsync(
        Guid userId,
        string email,
        IReadOnlyCollection<string> permissions,
        CancellationToken cancellationToken = default);
}