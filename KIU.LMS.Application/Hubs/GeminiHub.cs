namespace KIU.LMS.Application.Hubs;

public class GeminiHub : Hub
{
    private readonly IClaudeService _geminiService;
    public GeminiHub(IClaudeService geminiService)
    {
        _geminiService = geminiService;
    }

    //public async Task GenerateContent(string prompt)
    //{
    //    try
    //    {
    //        await _geminiService.StreamContentAsync(prompt, async (content) =>
    //        {
    //            await Clients.Caller.SendAsync("ReceiveContent", content);
    //        });

    //        await Clients.Caller.SendAsync("CompleteGeneration");
    //    }
    //    catch (Exception ex)
    //    {
    //        await Clients.Caller.SendAsync("ErrorOccurred", ex.Message);
    //    }
    //}

    public async Task GenerateContent(string prompt)
    {
        try
        {
            await _geminiService.GenerateStreamContentAsync(
                messages: null,
                prompt: prompt,
                handler: async (content) =>
                {
                    await Clients.Caller.SendAsync("ReceiveContent", content);
                }
            );

            await Clients.Caller.SendAsync("CompleteGeneration");
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ErrorOccurred", ex.Message);
        }
    }
}