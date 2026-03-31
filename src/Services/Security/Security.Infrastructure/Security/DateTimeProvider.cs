using Security.Application.Abstractions.Time;

namespace Security.Infrastructure.Security;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}