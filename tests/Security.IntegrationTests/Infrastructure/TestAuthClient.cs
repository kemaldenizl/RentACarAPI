using System.Net.Http.Headers;

namespace Security.IntegrationTests.Infrastructure;

public static class TestAuthClient
{
    public static void SetBearerToken(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public static void ClearBearerToken(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }
}