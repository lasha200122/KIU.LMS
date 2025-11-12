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
        string taskContent,
        int quantity,
        string difficulty)
    {
        var prompt = $@"
You are an expert exam-setter generating mathematics questions.

Your task:
- Generate **exactly {quantity}** new and distinct question variants.
- Each variant must be a NEW QUESTION (not a rephrase).
- Each variant must follow the formatting rules strictly.

Formatting rules:
1. Output exactly {quantity} numbered items: 1., 2., 3., ..., {quantity}.
2. Each item must have exactly these 6 lines:
   Question: <text>
   A) <correct answer>
   B) <distractor>
   C) <distractor>
   D) <distractor>
   explainCorrectAnswerDescription: <why A is correct>
   explainIncorrectAnswersDescription: <why B,C,D are wrong>
3. No additional text, notes, comments, or explanations.
4. Do NOT add more than {quantity} questions. If fewer are valid, stop exactly at that number.

Additional constraints:
- Option A must always be the correct answer.
- B, C, D must be plausible but incorrect.
- Each variant must test related but different aspects of the original question.
- Avoid rewording or trivial copies.

Domain: Mathematics (Linear Algebra)
Difficulty: {difficulty}

Generate exactly {quantity} valid questions based on:
{taskContent}";

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
        GeneratedQuestionDraft draft)
    {
        var prompt = $@"
You are an expert mathematics exam validator. Validate a single generated question.

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
- Must be Mathematics (Linear Algebra).
- Option A must be correct.
- Options B, C, D must be plausible but incorrect.
- If small errors present, provide corrected version in ""fixed"".
- If nonsense, off-topic or mathematically incorrect without fix → isValid=false.

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
                        @"You validate mathematics exam questions.
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