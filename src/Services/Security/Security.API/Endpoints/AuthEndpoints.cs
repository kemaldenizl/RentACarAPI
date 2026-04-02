using MediatR;
using Security.API.Abstractions;
using Security.API.Common;
using Security.API.Common.Auth;
using Security.API.Contracts.Auth;
using Security.API.Common.ErrorMapping;
using Security.API.Contracts.Errors;
using Security.Application.Auth.Login;
using Security.Application.Auth.Register;
using Security.Application.Auth.Refresh;
using Security.Application.Auth.Logout;
using Security.Application.Common.Results;
using Security.Infrastructure.RateLimiting;

namespace Security.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags(ApiTags.Auth);

        group.MapPost("/register", RegisterAsync)
            .RequireRateLimiting(RateLimitPolicyNames.Register)
            .WithName("Register")
            .WithSummary("Registers a new user.")
            .WithDescription("Creates a new user account.")
            .Accepts<RegisterRequest>("application/json")
            .Produces<Security.API.Contracts.Auth.RegisterResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .WithOpenApi();

        group.MapPost("/login", LoginAsync)
            .RequireRateLimiting(RateLimitPolicyNames.Login)
            .WithName("Login")
            .WithSummary("Authenticates a user.")
            .WithDescription("Authenticates a user with email and password and returns access and refresh tokens.")
            .Accepts<LoginRequest>("application/json")
            .Produces<Contracts.Auth.LoginResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .WithOpenApi();

        group.MapPost("/refresh", RefreshAsync)
            .RequireRateLimiting(RateLimitPolicyNames.Refresh)
            .WithName("RefreshToken")
            .WithSummary("Refreshes access and refresh tokens.")
            .WithDescription("Consumes a valid refresh token, rotates it, and returns a new access token and refresh token.")
            .Accepts<RefreshTokenRequest>("application/json")
            .Produces<Contracts.Auth.RefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .WithOpenApi();

        group.MapPost("/logout", LogoutAsync)
            .RequireRateLimiting(RateLimitPolicyNames.Logout)
            .RequireAuthorization()
            .WithName("Logout")
            .WithSummary("Logs out the current session.")
            .WithDescription("Revokes the current authenticated session.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .WithOpenApi();

        group.MapPost("/logout-all", LogoutAllAsync)
            .RequireRateLimiting(RateLimitPolicyNames.Logout)
            .RequireAuthorization()
            .WithName("LogoutAll")
            .WithSummary("Logs out all sessions.")
            .WithDescription("Revokes all sessions belonging to the current authenticated user.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password);

        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResult();
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password
        );

        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResult();
    }

    private static async Task<IResult> RefreshAsync(
        RefreshTokenRequest request,
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(
            request.RefreshToken
        );

        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResult();
    }

    private static async Task<IResult> LogoutAsync(
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var currentUser = httpContext.User.ToCurrentUser();
        var currentAccessToken = httpContext.User.ToCurrentAccessToken();

        if (currentUser.SessionId is null || string.IsNullOrWhiteSpace(currentUser.AccessTokenJti))
        {
            return Results.Problem(
                title: "Invalid session context",
                detail: "The current access token does not contain a valid session context.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var command = new LogoutCommand(
            currentUser.UserId,
            currentUser.SessionId.Value,
            currentUser.AccessTokenJti,
            currentAccessToken.ExpiresAtUtc
        );

        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResult();
    }

    private static async Task<IResult> LogoutAllAsync(
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var currentUser = httpContext.User.ToCurrentUser();
        var currentAccessToken = httpContext.User.ToCurrentAccessToken();

        if (string.IsNullOrWhiteSpace(currentUser.AccessTokenJti))
        {
            return Results.Problem(
                title: "Invalid token context",
                detail: "The current access token does not contain a valid JWT identifier.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var command = new LogoutAllCommand(
            currentUser.UserId,
            currentUser.AccessTokenJti,
            currentAccessToken.ExpiresAtUtc
        );

        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResult();
    }
}