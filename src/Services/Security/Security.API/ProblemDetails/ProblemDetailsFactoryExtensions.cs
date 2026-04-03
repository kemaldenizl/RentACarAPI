using Microsoft.AspNetCore.Mvc;

namespace Security.API.ProblemDetails;

public static class ProblemDetailsFactoryExtensions
{
    public static ProblemDetails CreateProblemDetails(
        this HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        string? type = null)
    {
        var problemDetails = new ProblemDetails
        {
            Type = type ?? ProblemDetailsDefaults.StatusType(statusCode),
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions[ProblemDetailsDefaults.CorrelationIdExtensionKey] =
            httpContext.TraceIdentifier;

        return problemDetails;
    }

    public static ValidationProblemDetails CreateValidationProblemDetails(
        this HttpContext httpContext,
        IDictionary<string, string[]> errors,
        string title = "Validation failed",
        string detail = "One or more validation errors occurred.")
    {
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Type = ProblemDetailsDefaults.StatusType(StatusCodes.Status400BadRequest),
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions[ProblemDetailsDefaults.CorrelationIdExtensionKey] =
            httpContext.TraceIdentifier;

        return problemDetails;
    }
}