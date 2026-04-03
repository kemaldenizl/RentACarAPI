using Microsoft.AspNetCore.Diagnostics;

namespace Security.API.ProblemDetails;

public sealed class ProblemDetailsExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = httpContext.CreateProblemDetails(
            StatusCodes.Status500InternalServerError,
            "Internal Server Error",
            "An unexpected error occurred.");

        await Results.Problem(problem).ExecuteAsync(httpContext);

        return true;
    }
}