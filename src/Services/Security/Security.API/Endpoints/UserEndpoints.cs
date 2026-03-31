using Security.API.Common;
using Security.API.Common.Auth;
using Security.API.Contracts.Auth;

namespace Security.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags(ApiTags.Users)
            .RequireAuthorization();

        group.MapGet("/me", (HttpContext httpContext) =>
        {
            var currentUser = httpContext.User.ToCurrentUser();

            var response = new CurrentUserResponse(
                currentUser.UserId,
                currentUser.Email,
                currentUser.Permissions
            );

            return Results.Ok(response);
        })
        .WithName("GetCurrentUser")
        .WithSummary("Gets the authenticated user.")
        .WithDescription("Returns the current authenticated user and resolved permissions from claims.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithOpenApi();
        
        return app;
    }
}