using Security.API.Common;
using Security.Domain.Authorization;

namespace Security.API.Endpoints;

public static class TestEndpoints
{
    public static IEndpointRouteBuilder MapTestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/test")
            .WithTags("Test")
            .RequireAuthorization();

        group.MapGet("/authenticated", () =>
            Results.Ok(new
            {
                message = "You are authenticated."
            }))
            .WithName("AuthenticatedTest")
            .WithSummary("Tests whether the user is authenticated.")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithOpenApi();

        group.MapGet("/users-read", () =>
            Results.Ok(new
            {
                message = "You have users.read permission."
            }))
            .RequireAuthorization(PermissionCodes.UsersRead)
            .WithName("UsersReadPermissionTest")
            .WithSummary("Tests whether the user has users.read permission.")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .WithOpenApi();

        return app;
    }
}