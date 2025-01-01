namespace KIU.LMS.Domain.Common.Models.Response;

public class Result
{
    protected Result(string message = "") => Message = message;

    protected Result(IEnumerable<Error> errors, string message = "") : this(message) => Errors = errors;

    protected Result(Error error, string message = "") : this(message) => Errors = [error];

    public bool IsSuccess => !Errors.Any();

    public string Message { get; protected set; }

    public IEnumerable<Error> Errors { get; protected set; } = [];

    public Error? FirstError => Errors.FirstOrDefault();

    public static Result Success(string message = "") => new([], message);

    public static Result Failure(Error error, string message = "") => new([error], message);

    public static Result Failure(IEnumerable<Error> errors, string message = "") => new(errors, message);

    public static Result Failure(string message) => new([Error.BadRequest], message);
}

public sealed class Result<T> : Result
{
    private Result(T data, string message = "") : base(message) => Data = data;

    private Result(T data, IEnumerable<Error> errors, string message = "") : base(errors, message) => Data = data;

    private Result(T data, Error error, string message = "") : base(error, message) => Data = data;

    public T? Data { get; }

    public static Result<T> Success(T data, string message = "") => new(data, [], message);

    public static new Result<T?> Failure(Error error, string message = "") => new(default, [error], message);

    public static new Result<T> Failure(string message) => new(default!, [Error.BadRequest], message);

    public static new Result<T?> Failure(IEnumerable<Error> errors, string message = "") => new(default, errors, message);

    public static implicit operator T?(Result<T> result) => result.Data;

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => Failure(error)!;
}
