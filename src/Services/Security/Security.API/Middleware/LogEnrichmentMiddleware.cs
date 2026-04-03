using System.Security.Claims;
using Serilog.Context;
using Security.Application.Common.Security;

namespace Security.API.Middleware;

public sealed class LogEnrichmentMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirstValue(CustomClaimTypes.Subject);
        var sessionId = context.User.FindFirstValue(CustomClaimTypes.SessionId);
        var accessTokenJti = context.User.FindFirstValue(CustomClaimTypes.JwtId);

        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        using (LogContext.PushProperty("RequestPath", context.Request.Path.Value ?? string.Empty))
        using (LogContext.PushProperty("HttpMethod", context.Request.Method))
        using (LogContext.PushProperty("UserId", userId ?? string.Empty))
        using (LogContext.PushProperty("SessionId", sessionId ?? string.Empty))
        using (LogContext.PushProperty("AccessTokenJti", accessTokenJti ?? string.Empty))
        {
            await next(context);
        }
    }
}