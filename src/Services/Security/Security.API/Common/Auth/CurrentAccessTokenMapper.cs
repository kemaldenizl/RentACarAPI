using System.Globalization;
using System.Security.Claims;
using Security.Application.Common.Security;

namespace Security.API.Common.Auth;

public static class CurrentAccessTokenMapper
{
    public static CurrentAccessToken ToCurrentAccessToken(this ClaimsPrincipal principal)
    {
        var jti = principal.FindFirstValue(CustomClaimTypes.JwtId);
        if (string.IsNullOrWhiteSpace(jti))
        {
            throw new InvalidOperationException("Authenticated access token does not contain a valid jti.");
        }

        var expClaim = principal.FindFirstValue("exp");
        if (string.IsNullOrWhiteSpace(expClaim))
        {
            throw new InvalidOperationException("Authenticated access token does not contain a valid exp claim.");
        }

        if (!long.TryParse(expClaim, NumberStyles.Integer, CultureInfo.InvariantCulture, out var expUnix))
        {
            throw new InvalidOperationException("Authenticated access token exp claim is invalid.");
        }

        var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

        return new CurrentAccessToken(jti, expiresAtUtc);
    }
}