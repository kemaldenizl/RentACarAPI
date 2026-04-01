namespace Security.Application.Abstractions.Security;

public interface IRefreshTokenGenerator
{
    (string PlainTextToken, string HashedToken) Generate();

    string Hash(string plainTextToken);
}