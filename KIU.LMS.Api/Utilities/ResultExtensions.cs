namespace KIU.LMS.Api.Utilities;

public static class ResultExtensions
{
    public static IResult ToResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return string.IsNullOrEmpty(result.Message) ? Results.Accepted() : Results.Ok(result.Message);
        }

        if (result.FirstError is null)
        {
            throw new InvalidOperationException();
        }

        return Results.Problem(
            statusCode: GetStatusCode(result.FirstError.Type),
            title: GetTitle(result.Message, result.FirstError.Type),
            type: GetType(result.FirstError.Type),
            extensions: new Dictionary<string, object?>
            {
                { "errors", result.Errors }
            }
        );
    }

    public static IResult ToResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Data);
        }

        if (result.FirstError is null)
        {
            throw new InvalidOperationException();
        }

        return Results.Problem(
            statusCode: GetStatusCode(result.FirstError.Type),
            title: GetTitle(result.Message, result.FirstError.Type),
            type: GetType(result.FirstError.Type),
            extensions: new Dictionary<string, object?>
            {
                { "errors", result.Errors }
            });
    }

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(string? resultMessage, ErrorType errorType)
    {
        if (!string.IsNullOrWhiteSpace(resultMessage))
        {
            return resultMessage;
        }

        return errorType switch
        {
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            _ => "Internal Server Error"
        };
    }

    private static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            ErrorType.Forbidden => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
            _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };
}
