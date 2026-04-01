using MediatR;
using Security.API.Abstractions;
using Security.API.Common;
using Security.API.Common.Auth;
using Security.API.Common.ErrorMapping;
using Security.API.Contracts.Sessions;
using Security.Application.Sessions.GetMySessions;
using Security.Application.Sessions.RevokeSession;
using Security.Infrastructure.RateLimiting;

namespace Security.API.Endpoints;

public static class SessionEndpoints
{
    public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sessions")
            .WithTags(ApiTags.Users)
            .RequireAuthorization();

        group.MapGet("/", GetMySessionsAsync)
            .RequireRateLimiting(RateLimitPolicyNames.Sessions)
            .WithName("GetMySessions")
            .WithSummary("Gets the current user's sessions.")
            .WithDescription("Returns active and revoked sessions belonging to the authenticated user.")
            .Produces<IReadOnlyCollection<SessionResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .WithOpenApi();

        group.MapDelete("/{id:guid}", RevokeSessionAsync)
            .RequireRateLimiting(RateLimitPolicyNames.Sessions)
            .WithName("RevokeSession")
            .WithSummary("Revokes a specific session.")
            .WithDescription("Revokes a specific session belonging to the authenticated user.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetMySessionsAsync(
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var currentUser = httpContext.User.ToCurrentUser();

        var query = new GetMySessionsQuery(
            currentUser.UserId,
            currentUser.SessionId);

        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.ToApiResult();

        var response = result.Value
            .Select(x => new SessionResponse(
                x.SessionId,
                x.DeviceName,
                x.IpAddress,
                x.CreatedAtUtc,
                x.Revoked,
                x.RevokedAtUtc,
                x.IsCurrent))
            .ToArray();

        return Results.Ok(response);
    }

    private static async Task<IResult> RevokeSessionAsync(
        Guid id,
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var currentUser = httpContext.User.ToCurrentUser();
        var currentAccessToken = httpContext.User.ToCurrentAccessToken();

        var command = new RevokeSessionCommand(
            currentUser.UserId,
            id,
            currentAccessToken.Jti,
            currentAccessToken.ExpiresAtUtc,
            currentUser.SessionId,
            httpContext.GetDeviceName(),
            httpContext.GetClientIpAddress());

        var result = await sender.Send(command, cancellationToken);

        return result.ToApiResult();
    }
}