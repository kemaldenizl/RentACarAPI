using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Security.Application.Abstractions.RequestContext;
using Security.Application.Common.Security;

namespace Security.Infrastructure.RequestContext;

public sealed class HttpRequestContext(IHttpContextAccessor httpContextAccessor) : IRequestContext
{
    private readonly HttpContext? _httpContext = httpContextAccessor.HttpContext;

    public string IpAddress
    {
        get
        {
            var forwardedFor = _httpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                var firstIp = forwardedFor.Split(',')[0].Trim();
                if (!string.IsNullOrWhiteSpace(firstIp))
                    return firstIp;
            }

            return _httpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    public string UserAgent
    {
        get
        {
            var userAgent = _httpContext?.Request.Headers.UserAgent.ToString();

            if (string.IsNullOrWhiteSpace(userAgent))
                return "unknown";

            return userAgent.Length > 2000
                ? userAgent[..2000]
                : userAgent;
        }
    }

    public string CorrelationId
    {
        get
        {
            var headerValue = _httpContext?.Request.Headers["X-Correlation-Id"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(headerValue))
                return headerValue!;

            return _httpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
        }
    }

    public string RequestPath => _httpContext?.Request.Path.Value ?? "unknown";

    public string HttpMethod => _httpContext?.Request.Method ?? "unknown";

    public Guid? UserId => ParseGuidClaim(CustomClaimTypes.Subject);

    public Guid? SessionId => ParseGuidClaim(CustomClaimTypes.SessionId);

    public string? AccessTokenJti => _httpContext?.User.FindFirstValue(CustomClaimTypes.JwtId);

    private Guid? ParseGuidClaim(string claimType)
    {
        var value = _httpContext?.User.FindFirstValue(claimType);
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }
}