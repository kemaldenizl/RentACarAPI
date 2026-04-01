using System.Security.Claims;

namespace Security.API.Abstractions;

internal static class RateLimitPartitionKeys
{
    public static string ByIp(HttpContext context, string endpointName)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var ip = !string.IsNullOrWhiteSpace(forwardedFor)
            ? forwardedFor.Split(',')[0].Trim()
            : context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return $"{endpointName}:ip:{ip}";
    }

    public static string ByAuthenticatedUserOrIp(HttpContext context, string endpointName)
    {
        var userId = context.User.FindFirstValue("sub");
        if (!string.IsNullOrWhiteSpace(userId))
        {
            return $"{endpointName}:user:{userId}";
        }

        return ByIp(context, endpointName);
    }
}