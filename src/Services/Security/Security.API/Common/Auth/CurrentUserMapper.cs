using System.Security.Claims;
using Security.Application.Common.Security;

namespace Security.API.Common.Auth;

public static class CurrentUserMapper
{
    public static CurrentUser ToCurrentUser(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue("sub");

        if (!Guid.TryParse(sub, out var userId))
            throw new InvalidOperationException("Authenticated user does not contain a valid subject identifier.");

        var email = principal.FindFirstValue("email") ?? string.Empty;

        var permissions = principal.FindAll(CustomClaimTypes.Permission)
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new CurrentUser(userId, email, permissions);
    }
}