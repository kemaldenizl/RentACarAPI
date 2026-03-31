namespace Security.API.Abstractions;

public static class HttpContextExtensions
{
    public static string GetClientIpAddress(this HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers.XForwardedFor.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var firstIp = forwardedFor.Split(',')[0].Trim();
            if (!string.IsNullOrWhiteSpace(firstIp))
                return firstIp;
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    public static string GetDeviceName(this HttpContext httpContext)
    {
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        if (string.IsNullOrWhiteSpace(userAgent))
            return "unknown-device";

        return userAgent.Length > 300
            ? userAgent[..300]
            : userAgent;
    }
}