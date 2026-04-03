using Microsoft.AspNetCore.Mvc;

namespace Security.API.ProblemDetails;

public static class ProblemDetailsExtensions
{
    public static IResult ToProblemResult(this HttpContext httpContext, Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails)
    {
        return Results.Problem(problemDetails);
    }

    public static IResult ToValidationProblemResult(this HttpContext httpContext, ValidationProblemDetails problemDetails)
    {
        return Results.ValidationProblem(problemDetails.Errors,
            statusCode: problemDetails.Status,
            title: problemDetails.Title,
            type: problemDetails.Type,
            detail: problemDetails.Detail,
            extensions: problemDetails.Extensions);
    }
}