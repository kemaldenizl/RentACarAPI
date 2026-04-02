using System.Text.Json;

namespace Security.Application.Common.Auditing;

public static class AuditPayloadBuilder
{
    public static string Build(object payload)
    {
        return JsonSerializer.Serialize(payload);
    }
}