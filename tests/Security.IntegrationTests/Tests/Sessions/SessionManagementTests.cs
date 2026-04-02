using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Security.IntegrationTests.Contracts.Auth;
using Security.IntegrationTests.Contracts.Sessions;
using Security.IntegrationTests.Fixtures;
using Security.IntegrationTests.Infrastructure;
using Xunit;

namespace Security.IntegrationTests.Tests.Sessions;

[Collection(IntegrationTestCollection.Name)]
public sealed class SessionManagementTests
{
    private readonly HttpClient _clientA;
    private readonly HttpClient _clientB;

    public SessionManagementTests(IntegrationTestFixture fixture)
    {
        var factory = new CustomWebApplicationFactory(fixture);

        _clientA = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        _clientB = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task User_Should_List_And_Revoke_Other_Session()
    {
        var email = $"sessions-{Guid.NewGuid():N}@example.com";
        const string password = "Str0ng!Password123";

        await _clientA.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest(email, password));

        var loginAResponse = await _clientA.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(email, password));

        var loginA = await loginAResponse.Content.ReadAsync<LoginResponse>();
        loginA.Should().NotBeNull();
        _clientA.SetBearerToken(loginA!.Tokens.AccessToken);

        var loginBResponse = await _clientB.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(email, password));

        var loginB = await loginBResponse.Content.ReadAsync<LoginResponse>();
        loginB.Should().NotBeNull();
        _clientB.SetBearerToken(loginB!.Tokens.AccessToken);

        var sessionsResponse = await _clientA.GetAsync("/api/sessions");
        sessionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var sessions = await sessionsResponse.Content.ReadAsync<SessionResponse[]>();
        sessions.Should().NotBeNull();
        sessions!.Length.Should().BeGreaterThanOrEqualTo(2);

        var currentSession = sessions.Single(x => x.IsCurrent);
        var otherSession = sessions.First(x => !x.IsCurrent);

        var revokeResponse = await _clientA.DeleteAsync($"/api/sessions/{otherSession.SessionId}");
        revokeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var refreshOtherResponse = await _clientB.PostAsJsonAsync("/api/auth/refresh",
            new RefreshTokenRequest(loginB.Tokens.RefreshToken));

        refreshOtherResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var meCurrentResponse = await _clientA.GetAsync("/api/users/me");
        meCurrentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        currentSession.SessionId.Should().NotBe(otherSession.SessionId);
    }
}