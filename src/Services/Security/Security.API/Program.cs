using Microsoft.EntityFrameworkCore;
using Security.Application;
using Security.Infrastructure;
using Security.Infrastructure.Persistence;
using Security.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SecurityDbContext>();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.SeedAsync();
}

app.MapOpenApi();
app.MapGet("/", () => Results.Ok(new { message = "Security service is running." }));

app.Run();

public partial class Program;