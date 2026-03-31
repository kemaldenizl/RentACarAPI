using Microsoft.AspNetCore.Identity;
using Security.Application.Abstractions.Security;

namespace Security.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(new object(), password);
    }

    public bool Verify(string passwordHash, string inputPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPassword);

        var result = _passwordHasher.VerifyHashedPassword(new object(), passwordHash, inputPassword);

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}