using System.Security.Claims;
using Security.Application.Common.Security;

namespace Security.API.Common.Auth;

public static class CurrentUserMapper
{
    public static CurrentUser ToCurrentUser(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(CustomClaimTypes.Subject);

        if (!Guid.TryParse(sub, out var userId))
            throw new InvalidOperationException("Authenticated user does not contain a valid subject identifier.");

        var email = principal.FindFirstValue(CustomClaimTypes.Email) ?? string.Empty;

        Guid? sessionId = null;
        var sid = principal.FindFirstValue(CustomClaimTypes.SessionId);
        if (Guid.TryParse(sid, out var parsedSessionId))
        {
            sessionId = parsedSessionId;
        }

        var jti = principal.FindFirstValue(CustomClaimTypes.JwtId);

        var permissions = principal.FindAll(CustomClaimTypes.Permission)
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new CurrentUser(userId, email, sessionId, jti, permissions);
    }
}