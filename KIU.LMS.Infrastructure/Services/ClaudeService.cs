using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

namespace KIU.LMS.Infrastructure.Services;

public class ClaudeService : IClaudeService
{
    private readonly ClaudeSettings _anthropicSettings;
    private readonly AnthropicClient _anthropicClient;

    public ClaudeService(ClaudeSettings anthropicSettings, AnthropicClient anthropicClient)
    {
        _anthropicSettings = anthropicSettings;
        _anthropicClient = anthropicClient;
    }

    public async Task<Message> GenerateContentAsync(
        List<Message> messages,
        List<SystemMessage> systemMessages)
    {
        var parameters = new MessageParameters
        {
            Messages = messages,
            MaxTokens = _anthropicSettings.MaxTokens,
            Model = AnthropicModels.Claude35Sonnet,
            Stream = false,
            Temperature = (decimal) _anthropicSettings.Temperature,
            System = systemMessages,
            PromptCaching = PromptCacheType.Messages
        };

        var response = await _anthropicClient.Messages.GetClaudeMessageAsync(parameters);
        return response.Message;
    }

    public async Task<string> GenerateStreamContentAsync(
        List<Message>? messages,
        string prompt,
        Action<string> handler,
        List<SystemMessage>? systemMessages = null)
    {
        messages ??= new List<Message>();
        messages.Add(new Message(RoleType.User, prompt));

        var parameters = new MessageParameters
        {
            Messages = messages,
            MaxTokens = _anthropicSettings.MaxTokens,
            Model = AnthropicModels.Claude35Sonnet,
            Stream = true,
            Temperature = (decimal) _anthropicSettings.Temperature,
            PromptCaching = PromptCacheType.Messages
        };

        string? lastMessage = null;
        await foreach (var response in _anthropicClient.Messages.StreamClaudeMessageAsync(parameters))
        {
            if (response.Delta is null)
                continue;

            if (!string.IsNullOrEmpty(response.Delta.Text))
            {
                handler(response.Delta.Text);
            }

            lastMessage = response.Delta.Text;
        }

        return lastMessage!;
    }
}
