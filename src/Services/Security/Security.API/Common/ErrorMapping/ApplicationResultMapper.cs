using Security.API.Contracts.Auth;
using Security.Application.Common.Results;
using AppLoginResponse = Security.Application.Auth.Login.LoginResponse;
using AppRegisterResponse = Security.Application.Auth.Register.RegisterResponse;

namespace Security.API.Common.ErrorMapping;

public static class ApplicationResultMapper
{
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
            var response = new Contracts.Auth.LoginResponse(
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

    private static IResult MapFailure<T>(Result<T> result)
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
}