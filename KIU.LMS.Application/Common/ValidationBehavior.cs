namespace KIU.LMS.Application.Common;

internal sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> _validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        List<ValidationFailure> failures = [];

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors);
            if (!result.IsValid)
            {
                break;
            }
        }

        var errors = failures.Select(x => Error.Validation(x.ErrorCode, x.ErrorMessage));

        if (failures.Count is not 0)
        {
            return HandleValidationErrors(failures, errors);
        }

        return await next();
    }

    private static TResponse HandleValidationErrors(List<ValidationFailure> failures, IEnumerable<Error> errors)
    {
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var invalidMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<int>.Failure), [typeof(IEnumerable<Error>), typeof(string)]);

            if (invalidMethod is not null)
            {
                return (TResponse)invalidMethod.Invoke(null, [errors, "Validation Error"])!;
            }
        }
        else if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errors, "Validation Error");
        }

        throw new ValidationException(failures);
    }
}
