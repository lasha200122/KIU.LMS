namespace KIU.LMS.Domain.Common.Models.Response;

public sealed record Error
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);
    public static readonly Error BadRequest = new("Error.Custom", "Check message for more information", ErrorType.BadRequest);

    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error NotFound(string code, string description)
    {
        return new(code, description, ErrorType.NotFound);
    }

    public static Error Validation(string code, string description)
    {
        return new(code, description, ErrorType.Validation);
    }

    public static Error Failure(string code, string description)
    {
        return new(code, description, ErrorType.Failure);
    }

    public static Error Conflict(string code, string description)
    {
        return new(code, description, ErrorType.Conflict);
    }

    public static Error Forbidden(string code, string description)
    {
        return new(code, description, ErrorType.Forbidden);
    }

    public static Error Unauthorized(string code, string description)
    {
        return new(code, description, ErrorType.Unauthorized);
    }

    public static Error General(string code = "General", string description = "Internal Server Error")
    {
        return new(code, description, ErrorType.General);
    }

    public static implicit operator Result(Error error) => Result.Failure(error);
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Forbidden = 4,
    Unauthorized = 5,
    BadRequest = 6,
    General = 99
}
