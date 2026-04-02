using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Security.API.Contracts.Health;

namespace Security.API.HealthChecks;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public static async Task WriteResponseAsync(HttpContext httpContext, HealthReport report)
    {
        httpContext.Response.ContentType = "application/json";

        var response = new HealthCheckResponse(
            report.Status.ToString(),
            report.TotalDuration,
            report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new HealthCheckEntryResponse(
                    entry.Value.Status.ToString(),
                    entry.Value.Description,
                    entry.Value.Duration,
                    entry.Value.Data.ToDictionary(data => data.Key,data => data.Value)
                )
            )
        );
                        
        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions));
    }
}