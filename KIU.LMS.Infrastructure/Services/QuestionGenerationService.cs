using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using KIU.LMS.Domain.Common.Models.Generating;
using KIU.LMS.Infrastructure.Common.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KIU.LMS.Infrastructure.Services;

public class QuestionGenerationService : IQuestionGenerationService
{
    private readonly ILogger<QuestionGenerationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly AIGradingConfiguration _config;

    public QuestionGenerationService(
        IHttpClientFactory httpClientFactory,
        IOptions<AIGradingConfiguration> config,
        ILogger<QuestionGenerationService> logger)
    {
        _logger = logger;
        _config = config.Value;
        _httpClient = httpClientFactory.CreateClient("ai-grader");
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);
    }
    
    public async Task<List<GeneratedQuestionDraft>?> GenerateAsync(
        string model,
        string prompt,
        int quantity,
        string difficulty)
    {
        var request = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = "You generate valid exam questions in strict format. No comments." },
                new { role = "user", content = prompt }
            },
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Generation failed. Model={Model}, Status={StatusCode}", model, response.StatusCode);
            return null;
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var raw = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        raw = raw
            .Trim()
            .Trim('`')
            .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        return QuestionParser.Parse(raw);
    }
    
    public async Task<QuestionValidationResult?> ValidateAsync(
        string model,
        string taskContent,
        GeneratedQuestionDraft draft)
    {
        var prompt = $@"
You are an expert exam validator. First, analyze the task content to identify the subject field, then validate the generated question.

Task Content:
{taskContent}

Based on the task content above, identify the academic subject/field this question should belong to.
Then validate the single generated question below.

Return ONLY strict JSON in this format:
{{
  ""isValid"": true/false,
  ""reason"": ""short text"",
  ""fixed"": {{
    ""question"": ""..."",
    ""A"": ""..."",
    ""B"": ""..."",
    ""C"": ""..."",
    ""D"": ""..."",
    ""explainCorrect"": ""..."",
    ""explainIncorrect"": ""...""
  }} or null
}}

Validation rules:
- Must match the subject field identified from the task content.
- Must align with the difficulty and topic requirements from the task content.
- Option A must be correct.
- Options B, C, D must be plausible but incorrect.
- If small errors present, provide corrected version in ""fixed"".
- If nonsense, off-topic or fundamentally incorrect without fix → isValid=false.

Question to validate:
Question: {draft.QuestionText}
A) {draft.OptionA}
B) {draft.OptionB}
C) {draft.OptionC}
D) {draft.OptionD}
explainCorrectAnswerDescription: {draft.ExplanationCorrect}
explainIncorrectAnswersDescription: {draft.ExplanationIncorrect}";

        var request = new
        {
            model = model,
            messages = new[]
            {
                new {
                    role = "system",
                    content = 
                        @"You validate exam questions across multiple academic fields.
First extract the subject field from the provided task content.
Then validate if the question matches that field and meets quality standards.
Return ONLY valid JSON with fields: isValid (boolean), reason (string), fixed (object or null).
DO NOT include <think>, chain-of-thought, natural language explanations, or markdown.
Answer strictly in JSON."
                },
                new { role = "user", content = prompt }
            },
            stream = false
        };


        var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Validation failed. Model={Model}, StatusCode={Status}", model, response.StatusCode);
            return null;
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var raw = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var finallRow = ExtractJson(raw)?.Trim()
            .Trim('`') 
            .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Replace("json", "")
            .Replace("\n", "", StringComparison.OrdinalIgnoreCase)
            .Trim();;
        
        return QuestionParser.ParseValidationJson(finallRow);
    }

    private static string? ExtractJson(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        raw = Regex.Replace(raw, "<think>[\\s\\S]*?</think>", "", RegexOptions.IgnoreCase);
        raw = raw.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "")
            .Trim();

        int start = raw.IndexOf('{');
        int end = raw.LastIndexOf('}');
        if (start == -1 || end == -1 || end <= start)
            return null;

        return raw.Substring(start, end - start + 1);
    }
}