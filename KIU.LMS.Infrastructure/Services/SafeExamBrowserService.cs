using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using KIU.LMS.Domain.Common.Interfaces.Services;
using KIU.LMS.Domain.Common.Models.SafeExamBrowser;

namespace KIU.LMS.Infrastructure.Services;

public class SafeExamBrowserService : ISafeExamBrowserService
{
    private const string RequestHashHeader = "X-SafeExamBrowser-RequestHash";
    private const string ConfigKeyHashHeader = "X-SafeExamBrowser-ConfigKeyHash";

    public string GenerateBrowserExamKey(Guid quizId, string salt)
    {
        var data = $"{quizId}{salt}{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public byte[] GenerateConfigurationFile(SebConfiguration config)
    {
        var plist = new XDocument(
            new XDocumentType("plist",
                "-//Apple Computer//DTD PLIST 1.0//EN",
                "http://www.apple.com/DTDs/PropertyList-1.0.dtd",
                null),
            new XElement("plist",
                new XAttribute("version", "1.0"),
                new XElement("dict",
                    // Start URL
                    new XElement("key", "startURL"),
                    new XElement("string", config.StartUrl),

                    // Send Browser Exam Key
                    new XElement("key", "sendBrowserExamKey"),
                    new XElement("true"),

                    // Browser Exam Key
                    new XElement("key", "browserExamKey"),
                    new XElement("string", config.BrowserExamKey),

                    // Quit Settings
                    new XElement("key", "allowQuit"),
                    new XElement(config.AllowQuit ? "true" : "false"),

                    new XElement("key", "quitURL"),
                    new XElement("string", config.StartUrl.Replace("/exam/", "/quiz/details/")),

                    new XElement("key", "quitURLConfirm"),
                    new XElement(config.QuitUrlConfirmation == 1 ? "true" : "false"),

                    // Security Settings - Disable Application Switching
                    new XElement("key", "enableSwitchToApplications"),
                    new XElement(config.EnableSwitchToApplications ? "true" : "false"),

                    new XElement("key", "allowSpellCheck"),
                    new XElement(config.AllowSpellCheck ? "true" : "false"),

                    new XElement("key", "showTaskBar"),
                    new XElement(config.ShowTaskBar ? "true" : "false"),

                    // Disable Function Keys
                    new XElement("key", "enableF1"),
                    new XElement("false"),

                    new XElement("key", "enableF2"),
                    new XElement("false"),

                    new XElement("key", "enableF3"),
                    new XElement("false"),

                    new XElement("key", "enableF4"),
                    new XElement("false"),

                    new XElement("key", "enableF5"),
                    new XElement("false"),

                    new XElement("key", "enableF6"),
                    new XElement("false"),

                    new XElement("key", "enableF7"),
                    new XElement("false"),

                    new XElement("key", "enableF8"),
                    new XElement("false"),

                    new XElement("key", "enableF9"),
                    new XElement("false"),

                    new XElement("key", "enableF10"),
                    new XElement("false"),

                    new XElement("key", "enableF11"),
                    new XElement("false"),

                    new XElement("key", "enableF12"),
                    new XElement("false"),

                    // Disable Escape and Control Keys
                    new XElement("key", "enableEsc"),
                    new XElement("false"),

                    new XElement("key", "enableCtrlEsc"),
                    new XElement("false"),

                    new XElement("key", "enableAltTab"),
                    new XElement("false"),

                    new XElement("key", "enableAltEsc"),
                    new XElement("false"),

                    new XElement("key", "enableAltF4"),
                    new XElement("false"),

                    new XElement("key", "enableStartMenu"),
                    new XElement("false"),

                    new XElement("key", "enableRightMouse"),
                    new XElement("false"),

                    // Browser Navigation
                    new XElement("key", "allowBrowsingBackForward"),
                    new XElement("true"),

                    new XElement("key", "newBrowserWindowByLinkBlockForeign"),
                    new XElement("true"),

                    new XElement("key", "enableZoomText"),
                    new XElement("true"),

                    new XElement("key", "enableZoomPage"),
                    new XElement("true"),

                    // Restart Exam URL
                    new XElement("key", "restartExamURL"),
                    new XElement("string", "")
                )
            )
        );

        using var memoryStream = new MemoryStream();
        plist.Save(memoryStream);
        return memoryStream.ToArray();
    }

    public SebRequestValidation ValidateRequest(
        string requestHash,
        string configKeyHash,
        string expectedBrowserExamKey,
        string url,
        string method)
    {
        if (string.IsNullOrWhiteSpace(requestHash))
            return SebRequestValidation.Failure("Missing X-SafeExamBrowser-RequestHash header");

        if (string.IsNullOrWhiteSpace(configKeyHash))
            return SebRequestValidation.Failure("Missing X-SafeExamBrowser-ConfigKeyHash header");

        // Validate Config Key Hash matches stored BEK
        if (!string.Equals(configKeyHash, expectedBrowserExamKey, StringComparison.OrdinalIgnoreCase))
            return SebRequestValidation.Failure("Invalid Browser Exam Key");

        // Validate Request Hash (BEK + URL + method)
        var computedRequestHash = ComputeRequestHash(expectedBrowserExamKey, url, method);
        if (!string.Equals(requestHash, computedRequestHash, StringComparison.OrdinalIgnoreCase))
            return SebRequestValidation.Failure("Invalid request hash");

        return SebRequestValidation.Success(requestHash, configKeyHash);
    }

    public bool IsRequestFromSafeBrowser(HttpContext context)
    {
        return context.Request.Headers.ContainsKey(RequestHashHeader) &&
               context.Request.Headers.ContainsKey(ConfigKeyHashHeader);
    }

    public (string? requestHash, string? configKeyHash) GetSebHeaders(HttpContext context)
    {
        var requestHash = context.Request.Headers[RequestHashHeader].FirstOrDefault();
        var configKeyHash = context.Request.Headers[ConfigKeyHashHeader].FirstOrDefault();
        return (requestHash, configKeyHash);
    }

    private string ComputeRequestHash(string browserExamKey, string url, string method)
    {
        // Request Hash = SHA256(BEK + URL + Method)
        var data = $"{browserExamKey}{url}{method}";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
