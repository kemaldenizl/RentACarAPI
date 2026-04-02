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
public sealed class RegisterLoginRefreshLogoutTests
{
    private readonly HttpClient _client;

    public RegisterLoginRefreshLogoutTests(IntegrationTestFixture fixture)
    {
        var factory = new CustomWebApplicationFactory(fixture);
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Register_Login_Refresh_Logout_Flow_Should_Work()
    {
        var email = $"user-{Guid.NewGuid():N}@example.com";
        const string password = "Str0ng!Password123";

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, password));

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var registered = await registerResponse.Content.ReadAsync<RegisterResponse>();
        registered.Should().NotBeNull();
        registered!.User.Email.Should().Be(email);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(email, password));

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await loginResponse.Content.ReadAsync<LoginResponse>();
        login.Should().NotBeNull();
        login!.Tokens.AccessToken.Should().NotBeNullOrWhiteSpace();
        login.Tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();

        var accessToken = login.Tokens.AccessToken;
        var refreshToken = login.Tokens.RefreshToken;

        _client.SetBearerToken(accessToken);

        var meResponse = await _client.GetAsync("/api/users/me");
        meResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var me = await meResponse.Content.ReadAsync<CurrentUserResponse>();
        me.Should().NotBeNull();
        me!.Email.Should().Be(email);
        me.SessionId.Should().NotBeNull();

        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenRequest(refreshToken));

        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshed = await refreshResponse.Content.ReadAsync<RefreshTokenResponse>();
        refreshed.Should().NotBeNull();
        refreshed!.Tokens.AccessToken.Should().NotBe(accessToken);
        refreshed.Tokens.RefreshToken.Should().NotBe(refreshToken);

        _client.SetBearerToken(refreshed.Tokens.AccessToken);

        var logoutResponse = await _client.PostAsync("/api/auth/logout", null);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterLogoutMe = await _client.GetAsync("/api/users/me");
        afterLogoutMe.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}