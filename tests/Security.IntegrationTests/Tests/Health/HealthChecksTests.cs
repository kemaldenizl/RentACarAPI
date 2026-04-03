using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Security.IntegrationTests.Contracts.Health;
using Security.IntegrationTests.Fixtures;
using Security.IntegrationTests.Infrastructure;
using Xunit;

namespace Security.IntegrationTests.Tests.Health;

[Collection(IntegrationTestCollection.Name)]
public sealed class HealthChecksTests
{
    private readonly HttpClient _client;

    public HealthChecksTests(IntegrationTestFixture fixture)
    {
        var factory = new CustomWebApplicationFactory(fixture);
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Live_HealthCheck_Should_Return_Healthy()
    {
        var response = await _client.GetAsync("/health/live");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadAsync<HealthCheckResponse>();
        payload.Should().NotBeNull();
        payload!.Status.Should().Be("Healthy");
        payload.Entries.Should().ContainKey("self");
    }

    [Fact]
    public async Task Ready_HealthCheck_Should_Return_Healthy()
    {
        var response = await _client.GetAsync("/health/ready");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadAsync<HealthCheckResponse>();
        payload.Should().NotBeNull();
        payload!.Status.Should().Be("Healthy");
        payload.Entries.Should().ContainKey("postgresql");
        payload.Entries.Should().ContainKey("redis");
    }
}