namespace KIU.LMS.Application.Hubs;

public class GeminiHub : Hub
{
    private readonly IGeminiService _geminiService;
    public GeminiHub(IGeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    public async Task GenerateContent(string prompt)
    {
        try
        {
            await _geminiService.StreamContentAsync(prompt, async (content) =>
            {
                await Clients.Caller.SendAsync("ReceiveContent", content);
            });

            await Clients.Caller.SendAsync("CompleteGeneration");
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ErrorOccurred", ex.Message);
        }
    }
}