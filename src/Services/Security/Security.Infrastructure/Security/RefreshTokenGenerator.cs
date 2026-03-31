using System.Security.Cryptography;
using System.Text;
using Security.Application.Abstractions.Security;

namespace Security.Infrastructure.Security;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public (string PlainTextToken, string HashedToken) Generate()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);

        var plainTextToken = Convert.ToBase64String(bytes);
        var hashedToken = HashToken(plainTextToken);

        return (plainTextToken, hashedToken);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}