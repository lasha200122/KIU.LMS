namespace KIU.LMS.Application.Common;

public class ExceptionLoggingHandler<TRequest, TResponse, TException>(Serilog.ILogger _logger) : IRequestExceptionHandler<TRequest, TResponse, TException>
         where TRequest : IRequest<TResponse>
         where TException : Exception
{
    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        _logger.Error(exception, "Something went wrong while handling request of type {RequestType}", typeof(TRequest));

        state.SetHandled(HandleError(exception)!);

        return Task.CompletedTask;
    }

    private static TResponse HandleError(Exception exception)
    {
        var error = Error.Failure("General.Error", "Something went wrong");

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var invalidMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<int>.Failure), new[] { typeof(Error), typeof(string) });

            if (invalidMethod is not null)
            {
                return (TResponse)invalidMethod.Invoke(null, new object[] { error, "Something went wrong" })!;
            }
        }

        return (TResponse)(object)Result.Failure(error);
    }
}