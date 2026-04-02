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

        // En sağlam yöntem: host build olmadan önce env var set et.
        Environment.SetEnvironmentVariable("ConnectionStrings__Postgres", _fixture.PostgresConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__Redis", _fixture.RedisConnectionString);

        Environment.SetEnvironmentVariable("Jwt__Issuer", "SecurityService.Tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "SecurityService.Tests.Clients");
        Environment.SetEnvironmentVariable("Jwt__SigningKey", "super-long-integration-test-signing-key-123456789");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenLifetimeMinutes", "15");

        Environment.SetEnvironmentVariable("IdentitySeed__Enabled", "true");
        Environment.SetEnvironmentVariable("IdentitySeed__AdminEmail", "admin@local");
        Environment.SetEnvironmentVariable("IdentitySeed__AdminPassword", "ChangeMe123!");

        Environment.SetEnvironmentVariable("RedisRevocation__InstanceName", "security-tests");
        Environment.SetEnvironmentVariable("RedisRevocation__AccessTokenRevocationPrefix", "revoked:access:jti:");

        Environment.SetEnvironmentVariable("RateLimiting__Register__PermitLimit", "3");
        Environment.SetEnvironmentVariable("RateLimiting__Register__WindowSeconds", "60");
        Environment.SetEnvironmentVariable("RateLimiting__Register__QueueLimit", "0");
        Environment.SetEnvironmentVariable("RateLimiting__Register__AutoReplenishment", "true");

        Environment.SetEnvironmentVariable("RateLimiting__Login__PermitLimit", "5");
        Environment.SetEnvironmentVariable("RateLimiting__Login__WindowSeconds", "60");
        Environment.SetEnvironmentVariable("RateLimiting__Login__QueueLimit", "0");
        Environment.SetEnvironmentVariable("RateLimiting__Login__AutoReplenishment", "true");

        Environment.SetEnvironmentVariable("RateLimiting__Refresh__PermitLimit", "10");
        Environment.SetEnvironmentVariable("RateLimiting__Refresh__WindowSeconds", "60");
        Environment.SetEnvironmentVariable("RateLimiting__Refresh__QueueLimit", "0");
        Environment.SetEnvironmentVariable("RateLimiting__Refresh__AutoReplenishment", "true");

        Environment.SetEnvironmentVariable("RateLimiting__Logout__PermitLimit", "10");
        Environment.SetEnvironmentVariable("RateLimiting__Logout__WindowSeconds", "60");
        Environment.SetEnvironmentVariable("RateLimiting__Logout__QueueLimit", "0");
        Environment.SetEnvironmentVariable("RateLimiting__Logout__AutoReplenishment", "true");

        Environment.SetEnvironmentVariable("RateLimiting__Sessions__PermitLimit", "20");
        Environment.SetEnvironmentVariable("RateLimiting__Sessions__WindowSeconds", "60");
        Environment.SetEnvironmentVariable("RateLimiting__Sessions__QueueLimit", "0");
        Environment.SetEnvironmentVariable("RateLimiting__Sessions__AutoReplenishment", "true");

        // İstersen bunu bırakabilirsin ama artık asıl yük env var'da.
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var config = new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = _fixture.PostgresConnectionString,
                ["ConnectionStrings:Redis"] = _fixture.RedisConnectionString
            };

            configBuilder.AddInMemoryCollection(config);
        });
    }
}