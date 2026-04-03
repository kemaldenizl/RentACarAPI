namespace Security.API.ProblemDetails;

public static class ProblemDetailsDefaults
{
    public const string CorrelationIdExtensionKey = "correlationId";
    public const string ErrorsExtensionKey = "errors";

    public static string StatusType(int statusCode) => $"https://httpstatuses.com/{statusCode}";
}