using MediatR;
using Security.API.Abstractions;
using Security.API.Common;
using Security.API.Common.ErrorMapping;
using Security.API.Contracts.Auth;
using Security.API.Contracts.Errors;
using Security.Application.Auth.Login;
using Security.Application.Auth.Register;
using Security.Application.Auth.Refresh;

namespace Security.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags(ApiTags.Auth);

        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .WithSummary("Registers a new user.")
            .WithDescription("Creates a new user account.")
            .Accepts<RegisterRequest>("application/json")
            .Produces<Security.API.Contracts.Auth.RegisterResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Authenticates a user.")
            .WithDescription("Authenticates a user with email and password and returns access and refresh tokens.")
            .Accepts<LoginRequest>("application/json")
            .Produces<Contracts.Auth.LoginResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapPost("/refresh", RefreshAsync)
            .WithName("RefreshToken")
            .WithSummary("Refreshes access and refresh tokens.")
            .WithDescription("Consumes a valid refresh token, rotates it, and returns a new access token and refresh token.")
            .Accepts<RefreshTokenRequest>("application/json")
            .Produces<Contracts.Auth.RefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
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
            request.Password,
            httpContext.GetDeviceName(),
            httpContext.GetClientIpAddress());

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
            request.RefreshToken,
            httpContext.GetDeviceName(),
            httpContext.GetClientIpAddress());

        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResult();
    }
}