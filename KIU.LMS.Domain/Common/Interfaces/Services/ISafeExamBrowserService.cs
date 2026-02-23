using KIU.LMS.Domain.Common.Models.SafeExamBrowser;

namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface ISafeExamBrowserService
{
    /// <summary>
    /// Generate Browser Exam Key (BEK) for a quiz
    /// </summary>
    string GenerateBrowserExamKey(Guid quizId, string salt);

    /// <summary>
    /// Generate SEB configuration file (.seb XML)
    /// </summary>
    byte[] GenerateConfigurationFile(SebConfiguration config);

    /// <summary>
    /// Validate SEB request headers
    /// </summary>
    SebRequestValidation ValidateRequest(
        string requestHash,
        string configKeyHash,
        string expectedBrowserExamKey,
        string url,
        string method);

    /// <summary>
    /// Check if request is from SEB browser
    /// </summary>
    bool IsRequestFromSafeBrowser(HttpContext context);

    /// <summary>
    /// Extract SEB headers from request
    /// </summary>
    (string? requestHash, string? configKeyHash) GetSebHeaders(HttpContext context);
}
