using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Security.IntegrationTests.Contracts.Auth;
using Security.IntegrationTests.Fixtures;
using Security.IntegrationTests.Infrastructure;
using Xunit;

namespace Security.IntegrationTests.Tests.Auth;

[Collection(IntegrationTestCollection.Name)]
public sealed class RefreshReuseDetectionTests
{
    private readonly HttpClient _client;

    public RefreshReuseDetectionTests(IntegrationTestFixture fixture)
    {
        var factory = new CustomWebApplicationFactory(fixture);
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Reusing_Consumed_Refresh_Token_Should_Revoke_Session()
    {
        var email = $"reuse-{Guid.NewGuid():N}@example.com";
        const string password = "Str0ng!Password123";

        await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, password));

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(email, password));

        var login = await loginResponse.Content.ReadAsync<LoginResponse>();
        login.Should().NotBeNull();

        var firstRefreshToken = login!.Tokens.RefreshToken;

        var firstRefreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenRequest(firstRefreshToken));

        firstRefreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var rotated = await firstRefreshResponse.Content.ReadAsync<RefreshTokenResponse>();
        rotated.Should().NotBeNull();

        var reuseResponse = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenRequest(firstRefreshToken));

        reuseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var secondTokenAfterReuseResponse = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenRequest(rotated!.Tokens.RefreshToken));

        secondTokenAfterReuseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}