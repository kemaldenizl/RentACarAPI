using Microsoft.EntityFrameworkCore;
using Security.API.Endpoints;
using Security.Application;
using Security.Infrastructure;
using Security.Infrastructure.Persistence;
using Security.Infrastructure.Persistence.Seed;
using Security.API.Extensions;
using Security.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimitExt(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.SeedAsync();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "Security.API",
    status = "running"
}))
.WithTags("System")
.WithOpenApi();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapSessionEndpoints();
app.MapTestEndpoints();

app.Run();

public partial class Program;