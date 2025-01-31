using System.Text.Json.Serialization;

namespace KIU.LMS.Domain.Common.Models.Gemini;

public class GradingResult
{
    [JsonPropertyName("grade")]
    public decimal grade { get; set; }

    [JsonPropertyName("feedback")]
    public string feedback { get; set; }

    [JsonPropertyName("suggestions")]
    public string suggestions { get; set; }

    public string GetFormattedFeedback()
    {
        return feedback?.Replace("\\n", Environment.NewLine) ?? string.Empty;
    }

    public string GetFormattedSuggestions()
    {
        return suggestions?.Replace("\\n", Environment.NewLine) ?? string.Empty;
    }
}