namespace KIU.LMS.Domain.Common.Models.SafeExamBrowser;

public sealed class SebRequestValidation
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }
    public string? RequestHash { get; }
    public string? ConfigKeyHash { get; }

    private SebRequestValidation(bool isValid, string? errorMessage, string? requestHash, string? configKeyHash)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        RequestHash = requestHash;
        ConfigKeyHash = configKeyHash;
    }

    public static SebRequestValidation Success(string requestHash, string configKeyHash)
        => new(true, null, requestHash, configKeyHash);

    public static SebRequestValidation Failure(string errorMessage)
        => new(false, errorMessage, null, null);
}
