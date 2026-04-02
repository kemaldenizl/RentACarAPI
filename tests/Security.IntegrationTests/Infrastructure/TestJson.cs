using System.Net.Http.Json;
using System.Text.Json;

namespace Security.IntegrationTests.Infrastructure;

public static class TestJson
{
    public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public static Task<T?> ReadAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
    {
        return content.ReadFromJsonAsync<T>(Default, cancellationToken);
    }
}