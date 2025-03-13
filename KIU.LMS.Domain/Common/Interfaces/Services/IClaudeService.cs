using Anthropic.SDK.Messaging;

namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IClaudeService
{
    Task<Message> GenerateContentAsync(List<Message> messages, List<SystemMessage>? systemMessages);
    Task<string> GenerateStreamContentAsync(List<Message>? messages, string prompt, Action<string> handler, List<SystemMessage>? systemMessages = null);
}
