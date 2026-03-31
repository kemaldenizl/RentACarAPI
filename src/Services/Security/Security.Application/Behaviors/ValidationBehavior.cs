using FluentValidation;
using MediatR;
using Security.Application.Common.Errors;
using Security.Application.Common.Results;

namespace Security.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(x => x.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(x => x.Errors)
            .Where(x => x is not null)
            .ToArray();

        if (failures.Length == 0)
            return await next();

        var message = string.Join(" | ", failures.Select(x => $"{x.PropertyName}: {x.ErrorMessage}"));
        var error = ValidationErrors.Invalid("request", message);

        object result = typeof(TResponse) switch
        {
            var type when type == typeof(Result) => Result.Failure(error),
            _ when typeof(TResponse).IsGenericType &&
                   typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>) =>
                CreateGenericFailure(error),
            _ => throw new InvalidOperationException(
                $"ValidationBehavior cannot create a failure response for {typeof(TResponse).Name}.")
        };

        return (TResponse)result;
    }

    private static object CreateGenericFailure(Error error)
    {
        var responseType = typeof(TResponse);
        var genericArgument = responseType.GetGenericArguments()[0];
        var resultType = typeof(Result<>).MakeGenericType(genericArgument);

        var method = resultType.GetMethod(nameof(Result<object>.Failure), [typeof(Error)])
                     ?? throw new InvalidOperationException("Failure factory method was not found.");

        return method.Invoke(null, [error])!;
    }
}