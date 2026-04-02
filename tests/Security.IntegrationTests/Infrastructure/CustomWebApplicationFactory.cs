using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Security.IntegrationTests.Fixtures;

namespace Security.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IntegrationTestFixture _fixture;

    public CustomWebApplicationFactory(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var config = new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = _fixture.PostgresConnectionString,
                ["ConnectionStrings:Redis"] = _fixture.RedisConnectionString,

                ["Jwt:Issuer"] = "SecurityService.Tests",
                ["Jwt:Audience"] = "SecurityService.Tests.Clients",
                ["Jwt:SigningKey"] = "super-long-integration-test-signing-key-123456789",
                ["Jwt:AccessTokenLifetimeMinutes"] = "15",

                ["IdentitySeed:Enabled"] = "true",
                ["IdentitySeed:AdminEmail"] = "admin@local",
                ["IdentitySeed:AdminPassword"] = "ChangeMe123!",

                ["RedisRevocation:InstanceName"] = "security-tests",
                ["RedisRevocation:AccessTokenRevocationPrefix"] = "revoked:access:jti:",

                ["RateLimiting:Register:PermitLimit"] = "3",
                ["RateLimiting:Register:WindowSeconds"] = "60",
                ["RateLimiting:Register:QueueLimit"] = "0",
                ["RateLimiting:Register:AutoReplenishment"] = "true",

                ["RateLimiting:Login:PermitLimit"] = "5",
                ["RateLimiting:Login:WindowSeconds"] = "60",
                ["RateLimiting:Login:QueueLimit"] = "0",
                ["RateLimiting:Login:AutoReplenishment"] = "true",

                ["RateLimiting:Refresh:PermitLimit"] = "10",
                ["RateLimiting:Refresh:WindowSeconds"] = "60",
                ["RateLimiting:Refresh:QueueLimit"] = "0",
                ["RateLimiting:Refresh:AutoReplenishment"] = "true",

                ["RateLimiting:Logout:PermitLimit"] = "10",
                ["RateLimiting:Logout:WindowSeconds"] = "60",
                ["RateLimiting:Logout:QueueLimit"] = "0",
                ["RateLimiting:Logout:AutoReplenishment"] = "true",

                ["RateLimiting:Sessions:PermitLimit"] = "20",
                ["RateLimiting:Sessions:WindowSeconds"] = "60",
                ["RateLimiting:Sessions:QueueLimit"] = "0",
                ["RateLimiting:Sessions:AutoReplenishment"] = "true"
            };

            configBuilder.AddInMemoryCollection(config);
        });
    }
}