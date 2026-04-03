using Security.API.Contracts.Auth;
using Security.API.ProblemDetails;
using Security.Application.Common.Results;
using AppLoginResponse = Security.Application.Auth.Login.LoginResponse;
using AppRefreshTokenResponse = Security.Application.Auth.Refresh.RefreshTokenResponse;
using AppRegisterResponse = Security.Application.Auth.Register.RegisterResponse;

namespace Security.API.Common.ErrorMapping;

public static class ApplicationResultMapper
{
    public static IResult ToApiResult(this HttpContext httpContext, Result result)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MapFailure(httpContext, result);
    }

    public static IResult ToApiResult(this HttpContext httpContext, Result<AppRegisterResponse> result)
    {
        if (result.IsSuccess)
        {
            var response = new RegisterResponse(
                new UserResponse(
                    result.Value.User.Id,
                    result.Value.User.Email,
                    result.Value.User.EmailVerified,
                    result.Value.User.IsActive));

            return Results.Created($"/api/users/{response.User.Id}", response);
        }

        return MapFailure(httpContext, result);
    }

    public static IResult ToApiResult(this HttpContext httpContext, Result<AppLoginResponse> result)
    {
        if (result.IsSuccess)
        {
            var response = new LoginResponse(
                new UserResponse(
                    result.Value.User.Id,
                    result.Value.User.Email,
                    result.Value.User.EmailVerified,
                    result.Value.User.IsActive),
                new AuthTokensResponse(
                    result.Value.Tokens.AccessToken,
                    result.Value.Tokens.AccessTokenExpiresAtUtc,
                    result.Value.Tokens.RefreshToken,
                    result.Value.Tokens.RefreshTokenExpiresAtUtc));

            return Results.Ok(response);
        }

        return MapFailure(httpContext, result);
    }

    public static IResult ToApiResult(this HttpContext httpContext, Result<AppRefreshTokenResponse> result)
    {
        if (result.IsSuccess)
        {
            var response = new RefreshTokenResponse(
                new AuthTokensResponse(
                    result.Value.Tokens.AccessToken,
                    result.Value.Tokens.AccessTokenExpiresAtUtc,
                    result.Value.Tokens.RefreshToken,
                    result.Value.Tokens.RefreshTokenExpiresAtUtc));

            return Results.Ok(response);
        }

        return MapFailure(httpContext, result);
    }

    private static IResult MapFailure(Result result)
    {
        throw new InvalidOperationException("HttpContext is required for problem details mapping.");
    }

    private static IResult MapFailure<T>(HttpContext httpContext, Result<T> result)
    {
        return MapFailure(httpContext, (Result)result);
    }

    private static IResult MapFailure(HttpContext httpContext, Result result)
    {
        var errorCode = result.Error.Code;

        return errorCode switch
        {
            "validation.invalid" => httpContext.ToValidationProblemResult(
                httpContext.CreateValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        ["request"] = [result.Error.Description]
                    })),

            "auth.invalid_credentials" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Authentication failed",
                    result.Error.Description)),

            "auth.invalid_refresh_token" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Invalid refresh token",
                    result.Error.Description)),

            "auth.expired_refresh_token" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Expired refresh token",
                    result.Error.Description)),

            "auth.revoked_refresh_token" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Revoked refresh token",
                    result.Error.Description)),

            "auth.consumed_refresh_token" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Consumed refresh token",
                    result.Error.Description)),

            "auth.session_revoked" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Session revoked",
                    result.Error.Description)),

            "auth.refresh_token_reuse_detected" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Refresh token reuse detected",
                    result.Error.Description)),

            "auth.invalid_session" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Invalid session",
                    result.Error.Description)),

            "auth.session_not_found" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Session not found",
                    result.Error.Description)),

            "auth.user_inactive" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status403Forbidden,
                    "User inactive",
                    result.Error.Description)),

            "auth.email_not_verified" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status403Forbidden,
                    "Email is not verified",
                    result.Error.Description)),

            "auth.user_already_exists" => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    result.Error.Description)),

            _ => httpContext.ToProblemResult(
                httpContext.CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Application error",
                    result.Error.Description))
        };
    }
}