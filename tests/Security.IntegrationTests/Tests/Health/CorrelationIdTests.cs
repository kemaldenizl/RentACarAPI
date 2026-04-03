using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Security.IntegrationTests.Fixtures;
using Security.IntegrationTests.Infrastructure;
using Xunit;

namespace Security.IntegrationTests.Tests.Health;

[Collection(IntegrationTestCollection.Name)]
public sealed class CorrelationIdTests
{
    private readonly HttpClient _client;

    public CorrelationIdTests(IntegrationTestFixture fixture)
    {
        var factory = new CustomWebApplicationFactory(fixture);
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Health_Endpoint_Should_Return_CorrelationId_Header()
    {
        const string correlationId = "test-correlation-id-123";

        var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.Add("X-Correlation-Id", correlationId);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().Contain(h => h.Key == "X-Correlation-Id");
        response.Headers.GetValues("X-Correlation-Id").Single().Should().Be(correlationId);
    }
}