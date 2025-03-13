namespace KIU.LMS.Domain.Common.Settings.Gemini;

public class ClaudeSettings
{
    public required string ApiKey { get; set; }
    public string Model { get; set; } = "claude-3-sonnet-20240229";
    public int MaxTokens { get; set; } = 4000;
    public double Temperature { get; set; } = 0.7;
}
