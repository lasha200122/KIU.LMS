namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IGeminiService
{
    Task<string?> GenerateContentAsync(string prompt);
    Task<string> GenerateContentWithImageAsync(string prompt, byte[] imageData, string fileName);
    Task<List<string>> GenerateChatAsync(List<string> conversation);
    Task StreamContentAsync(string prompt, Action<string> handler);
}