using Security.Domain.Exceptions;

namespace Security.Domain.Common;

public static class Guard
{
    public static string AgainstNullOrWhiteSpace(string? input, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException($"{fieldName} cannot be empty.");

        return input.Trim();
    }

    public static Guid AgainstEmpty(Guid value, string fieldName)
    {
        if (value == Guid.Empty)
            throw new DomainException($"{fieldName} cannot be empty.");

        return value;
    }

    public static DateTime AgainstDefault(DateTime value, string fieldName)
    {
        if (value == default)
            throw new DomainException($"{fieldName} cannot be default.");

        return value;
    }

    public static void Against(bool condition, string message)
    {
        if (condition)
            throw new DomainException(message);
    }
}