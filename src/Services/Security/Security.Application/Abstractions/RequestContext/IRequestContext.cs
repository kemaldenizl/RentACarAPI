namespace Security.Application.Abstractions.RequestContext;

public interface IRequestContext
{
    string IpAddress { get; }
    string UserAgent { get; }
    string CorrelationId { get; }
    string RequestPath { get; }
    string HttpMethod { get; }

    Guid? UserId { get; }
    Guid? SessionId { get; }
    string? AccessTokenJti { get; }
}