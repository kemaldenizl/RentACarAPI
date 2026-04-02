using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Security.IntegrationTests.Contracts.Auth;
using Security.IntegrationTests.Fixtures;
using Xunit;
using Security.IntegrationTests.Contracts.Sessions;
using Security.IntegrationTests.Infrastructure;

namespace Security.IntegrationTests.Tests.RateLimiting;

[Collection(IntegrationTestCollection.Name)]
public sealed class RateLimitingTests
{
    private readonly HttpClient _client;

    public RateLimitingTests(IntegrationTestFixture fixture)
    {
        var factory = new CustomWebApplicationFactory(fixture);
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Login_Should_Return_429_When_Rate_Limit_Is_Exceeded()
    {
        const string email = "admin@local";
        const string wrongPassword = "wrong-password";

        HttpResponseMessage? lastResponse = null;

        for (var i = 0; i < 6; i++)
        {
            lastResponse = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginRequest(email, wrongPassword));
        }

        lastResponse.Should().NotBeNull();
        lastResponse!.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        lastResponse.Headers.Should().ContainKey("Retry-After");
    }
}