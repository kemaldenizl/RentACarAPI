using Security.API.Contracts.Auth;
using Security.Application.Common.Results;
using AppLoginResponse = Security.Application.Auth.Login.LoginResponse;
using AppRefreshTokenResponse = Security.Application.Auth.Refresh.RefreshTokenResponse;
using AppRegisterResponse = Security.Application.Auth.Register.RegisterResponse;

namespace Security.API.Common.ErrorMapping;

public static class ApplicationResultMapper
{
    public static IResult ToApiResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        return MapFailure(result);
    }

    public static IResult ToApiResult(this Result<AppRegisterResponse> result)
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

        return MapFailure(result);
    }

    public static IResult ToApiResult(this Result<AppLoginResponse> result)
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

        return MapFailure(result);
    }

    public static IResult ToApiResult(this Result<AppRefreshTokenResponse> result)
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

        return MapFailure(result);
    }

    private static IResult MapFailure(Result result)
    {
        var errorCode = result.Error.Code;

        return errorCode switch
        {
            "validation.invalid" => Results.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["request"] = [result.Error.Description]
                },
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation failed",
                detail: result.Error.Description),

            "auth.invalid_credentials" => Results.Problem(
                title: "Authentication failed",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status401Unauthorized),

            "auth.invalid_refresh_token" => Results.Problem(
                title: "Invalid refresh token",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status401Unauthorized),

            "auth.expired_refresh_token" => Results.Problem(
                title: "Expired refresh token",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status401Unauthorized),

            "auth.revoked_refresh_token" => Results.Problem(
                title: "Revoked refresh token",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status401Unauthorized),

            "auth.consumed_refresh_token" => Results.Problem(
                title: "Consumed refresh token",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status401Unauthorized),

            "auth.session_revoked" => Results.Problem(
                title: "Session revoked",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status401Unauthorized),

            "auth.refresh_token_reuse_detected" => Results.Problem(
                title: "Refresh token reuse detected",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status401Unauthorized),

            "auth.invalid_session" => Results.Problem(
                title: "Invalid session",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status400BadRequest),

            "auth.session_not_found" => Results.Problem(
                title: "Session not found",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status404NotFound),

            "auth.user_inactive" => Results.Problem(
                title: "User inactive",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status403Forbidden),

            "auth.email_not_verified" => Results.Problem(
                title: "Email is not verified",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status403Forbidden),

            "auth.user_already_exists" => Results.Problem(
                title: "Conflict",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status409Conflict),

            _ => Results.Problem(
                title: "Application error",
                detail: result.Error.Description,
                statusCode: StatusCodes.Status400BadRequest)
        };
    }

    private static IResult MapFailure<T>(Result<T> result)
    {
        return MapFailure((Result)result);
    }
}