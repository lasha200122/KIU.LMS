namespace KIU.LMS.Infrastructure.Services;

public class GeminiService : IGeminiService
{
    private readonly GenerativeModel _model;
    private readonly GeminiProVision _visionModel;

    public GeminiService(GeminiSettings _settings)
    {
        _model = new GenerativeModel(_settings.ApiKey);
        _visionModel = new GeminiProVision(_settings.ApiKey);
    }

    public async Task<string?> GenerateContentAsync(string prompt)
    {
            return await _model.GenerateContentAsync(prompt);
    }

    public async Task<string> GenerateContentWithImageAsync(string prompt, byte[] imageData, string fileName)
    {
            var fileObject = new FileObject(imageData, fileName);
            var response = await _visionModel.GenerateContentAsync(prompt, fileObject);
            return response.Text()!;
    }

    public async Task<List<string>> GenerateChatAsync(List<string> conversation)
    {
            var chat = _model.StartChat(new StartChatParams());
            var responses = new List<string>();

            foreach (var message in conversation)
            {
                var response = await chat.SendMessageAsync(message);
                responses.Add(response);
            }

            return responses;
    }

    public async Task StreamContentAsync(string prompt, Action<string> handler)
    {
            await _model.StreamContentAsync(prompt, handler);
    }
}
