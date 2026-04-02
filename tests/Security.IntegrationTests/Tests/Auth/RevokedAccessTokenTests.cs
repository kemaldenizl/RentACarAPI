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
public sealed class RevokedAccessTokenTests
{
    private readonly HttpClient _client;

    public RevokedAccessTokenTests(IntegrationTestFixture fixture)
    {
        var factory = new CustomWebApplicationFactory(fixture);
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Logout_Should_Revoke_Current_Access_Token()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("admin@local", "ChangeMe123!"));

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await loginResponse.Content.ReadAsync<LoginResponse>();
        login.Should().NotBeNull();

        _client.SetBearerToken(login!.Tokens.AccessToken);

        var meBeforeLogout = await _client.GetAsync("/api/users/me");
        meBeforeLogout.StatusCode.Should().Be(HttpStatusCode.OK);

        var logoutResponse = await _client.PostAsync("/api/auth/logout", null);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var meAfterLogout = await _client.GetAsync("/api/users/me");
        meAfterLogout.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}