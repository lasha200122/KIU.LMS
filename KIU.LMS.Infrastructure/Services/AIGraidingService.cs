using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using KIU.LMS.Infrastructure.Common;
using KIU.LMS.Infrastructure.Common.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KIU.LMS.Infrastructure.Services;

public class AIGradingService : IAiGradingService
{
    private readonly ILogger<AIGradingService> _logger;
    private readonly HttpClient _httpClient;
    private readonly AIGradingConfiguration aiGradingConfiguration;

    public AIGradingService(
        IHttpClientFactory httpClientFactory,
        IOptions<AIGradingConfiguration> configuration,
        ILogger<AIGradingService> logger)
    {
        _logger = logger;
        aiGradingConfiguration = configuration.Value;
        _httpClient = httpClientFactory.CreateClient("ai-grader");
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", aiGradingConfiguration.ApiKey);
    }

    public async Task<string?> GradeAsync(
        string problem, string referenceSolution, 
        string studentSubmission, string rubric, decimal score)
    {
        var request = new
        {
            model = aiGradingConfiguration.AIModel,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content =
                        "You are an expert grader. Grade strictly based on the problem and reference solution. Output ONLY JSON with keys \"grade\" and \"feedback\"."
                },
                new
                {
                    role = "user",
                    content =
                        $@"You are an expert grader. Use the provided grading prompt/rubric. Grade strictly by the problem and the reference solution.

Return ONLY strict JSON with keys grade and feedback, no code fences, no extra text.

Constraints:
- grade must be a number between 0 and {score}
- feedback must be concise, but if the solution is correct or nearly correct, do not respond with a single word like 'Good'. Provide a short meaningful comment (1–2 sentences) describing what was done well.

Grading Prompt (rubric):
{rubric}

Inputs:
- Problem:
{problem}
- Reference solution:
{referenceSolution}
- Student submission:
{studentSubmission}

Output format example:
{{""grade"": 10, ""feedback"": ""Solution is correct and clearly explained.""}}"
                }
            },
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
       
        return content
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?.Trim()
            .Trim('`')   
            .Replace("json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("\n", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
    }

    public async Task<(bool isValid, string? reason)> ValidateAsync(string problem, string referenceSolution, string studentSubmission,
        string rubric, string gradeResultJson)
    {
        var request = new
        {
            model = aiGradingConfiguration.AIModel,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = @"
You are an expert in grading validation. 
Your goal is to check whether the **AI's grading decision makes logical and contextual sense** based on the given data.

Return ONLY valid JSON with exactly two keys: ""isValid"" (boolean) and ""reason"" (string).
Never include explanations, code fences, or extra text.

Decision rules:
1. Return ""isValid"": false ONLY IF:
   - The student's answer or AI grading result is **nonsensical, off-topic, unrelated to the problem**, or **contains random, incoherent text**.
   - Example: The student writes 'blue apple run fast' or irrelevant unrelated sentences.

2. Return ""isValid"": true IF:
   - The student's answer has any logical or partial connection to the problem.
   - The AI's grading decision is based on actual reasoning related to the rubric or problem.
   - Minor factual, grammatical, or reasoning mistakes do NOT make it invalid.

3. Your role is to detect nonsense or disconnected content — not to grade correctness.

Examples:
✅ A coherent but partially wrong answer → { ""isValid"": true, ""reason"": ""The answer is meaningful and relevant."" }
❌ A random, off-topic, or incoherent answer → { ""isValid"": false, ""reason"": ""The answer is nonsensical or unrelated."" }"
                },
                new
                {
                    role = "user",
                    content = $@"
Evaluate the following grading decision for coherence and meaning.

Original Problem:
{problem}

Reference Solution:
{referenceSolution}

Student Submission:
{studentSubmission}

Grading Rubric:
{rubric}

AI Grading Result:
{gradeResultJson}

Output format:
{{""isValid"":true,""reason"":""ok""}} or {{""isValid"":false,""reason"":""Answer is nonsensical or unrelated.""}}"
                }
            },
            stream = false
        };
        
        var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
        if (!response.IsSuccessStatusCode)
            return (false, null);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();

        _logger.LogInformation("Validation output: {Output}", JsonSerializer.Serialize(content));
        
        var rawMessage = content
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;

        var cleaned = rawMessage
            .Trim()
            .Trim('`')   
            .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (cleaned.StartsWith("json", StringComparison.OrdinalIgnoreCase))
            cleaned = cleaned.Substring(4).Trim();

        var startIndex = cleaned.IndexOf('{');
        if (startIndex > 0)
            cleaned = cleaned.Substring(startIndex);

        var endIndex = cleaned.LastIndexOf('}');
        if (endIndex > 0 && endIndex < cleaned.Length - 1)
            cleaned = cleaned.Substring(0, endIndex + 1);

        using var doc = JsonDocument.Parse(cleaned);

        var root = doc.RootElement;
        var isValid = !root.TryGetProperty("isValid", out var isValidProp) && false;
        var reason = root.GetProperty("reason").GetString();
        return (isValid, reason);
    }
    
    public async Task<string?> GenerateCodeFromPromptAsync(string prompt)
    {
        var request = new
        {
            model = aiGradingConfiguration.AIModel,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are an expert programmer. Generate clean, working code based on the user's description. Return ONLY the code, no explanations."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
   
        return content
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?.Trim();
    }

}