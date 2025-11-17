using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using KIU.LMS.Domain.Common.Enums.Assignment;
using KIU.LMS.Infrastructure.Common.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KIU.LMS.Infrastructure.Services;

public class TaskGenerationService : ITaskGenerationService
{
    private readonly ILogger<TaskGenerationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly AIGradingConfiguration _config;

    public TaskGenerationService(
        IHttpClientFactory httpClientFactory,
        IOptions<AIGradingConfiguration> config,
        ILogger<TaskGenerationService> logger)
    {
        _logger = logger;
        _config = config.Value;
        _httpClient = httpClientFactory.CreateClient("ai-grader");
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.ApiKey);
    }
    
    public async Task<List<GeneratedTaskDraft>?> GenerateAsync(
        string model,
        string taskContent,
        int quantity,
        DifficultyType difficulty,
        GeneratedAssignmentType type)
    {
        var prompt = BuildGenerationPrompt(taskContent, quantity, difficulty, type);
        
        var request = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = "You generate valid coding/problem-solving tasks in strict format. No comments." },
                new { role = "user", content = prompt }
            },
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Task generation failed. Model={Model}, Status={StatusCode}", model, response.StatusCode);
            return null;
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var raw = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        raw = raw
            .Trim()
            .Trim('`')
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        return TaskParser.Parse(raw, type);
    }
    
    public async Task<TaskValidationResult?> ValidateAsync(
        string model,
        string taskContent,
        GeneratedTaskDraft draft,
        GeneratedAssignmentType type)
    {
        var prompt = BuildValidationPrompt(taskContent, draft, type);

        var request = new
        {
            model = model,
            messages = new[]
            {
                new {
                    role = "system",
                    content = 
                        @"You validate coding/problem-solving tasks across multiple programming domains.
First extract the subject field and technical requirements from the provided task content.
Then validate if the task matches those requirements and meets quality standards.
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
            _logger.LogError("Task validation failed. Model={Model}, StatusCode={Status}", model, response.StatusCode);
            return null;
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var raw = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var finalRow = ExtractJson(raw)?.Trim()
            .Trim('`') 
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Replace("json", "")
            .Replace("\n", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
        
        return TaskParser.ParseValidationJson(finalRow, type);
    }

    private string BuildGenerationPrompt(string taskContent, int quantity, DifficultyType difficulty, GeneratedAssignmentType type)
    {
        var basePrompt = $@"
You are an expert exam-setter who generates coding/problem-solving task variants.

RULES:
0) Do not write thinking text
1) Each variant must be a NEW TASK with different requirements or context
2) Test related concepts or nearby problems
3) The correct solution must change accordingly
4) Solutions can be code OR step-by-step problem solutions depending on the task type
5) Output MUST be concise and structured
6) No preambles or epilogues; return only the generated variants

Difficulty: {(int)difficulty}
Quantity: {quantity}
Task Type: {type}

Create {quantity} distinct variants of this task:

{taskContent}

Formatting requirements:
- Use a clear numbered list: 1., 2., 3., ...
- For each variant return the following fields in order:";

        if (type == GeneratedAssignmentType.C2RS)
        {
            basePrompt += @"
  Task Description: <detailed coding task description>
  Code Solution: <complete working code that solves the task>
  Code Generation Prompt: LEAVE_EMPTY
  Code Grading Prompt: <detailed prompt explaining how to compare student code with reference solution, including key criteria like correctness, efficiency, code style, edge cases handling, and point allocation>
  Difficulty: [same as input difficulty]";
        }
        else if (type == GeneratedAssignmentType.IPEQ)
        {
            basePrompt += @"
  Task Description: <detailed task description for prompt engineering challenge>
  Code Solution: <example of correct code that should be generated>
  Code Generation Prompt: <example prompt that would generate similar correct code - student must write similar quality prompt>
  Code Grading Prompt: <detailed prompt explaining how to evaluate both the student's prompt quality and the code it generates, including clarity, specificity, correctness of generated output, and point allocation>
  Difficulty: [same as input difficulty]";
        }

        basePrompt += @"

- Separate each variant with a blank line
- NO explanations or comments needed beyond the required fields";

        return basePrompt;
    }

    private string BuildValidationPrompt(string taskContent, GeneratedTaskDraft draft, GeneratedAssignmentType type)
    {
        var typeDescription = type == GeneratedAssignmentType.C2RS 
            ? "C2RS (Compare to Reference Solution) - student writes code compared to reference"
            : "IPEQ (Iterative Prompt Engineering Quality) - student writes prompt to generate code";

        return $@"
You are an expert coding task validator. First, analyze the task content to identify the programming domain and requirements, then validate the generated task.

Task Type: {typeDescription}

Task Content:
{taskContent}

Based on the task content above, identify the programming language, domain, and technical requirements.
Then validate the single generated task below.

Return ONLY strict JSON in this format:
{{
  ""isValid"": true/false,
  ""reason"": ""short text"",
  ""fixed"": {{
    ""taskDescription"": ""..."",
    ""codeSolution"": ""..."",
    ""codeGenerationPrompt"": ""..."",
    ""codeGradingPrompt"": ""...""
  }} or null
}}

Validation rules:
- Must match the programming domain and requirements from task content
- Code solution must be syntactically correct and solve the task
- For C2RS: codeGenerationPrompt must be empty, grading prompt must focus on comparison with reference
- For IPEQ: codeGenerationPrompt must be clear and specific, grading prompt must evaluate both prompt quality and generated code
- Grading prompt must include clear evaluation criteria and point allocation
- If small errors present, provide corrected version in ""fixed""
- If nonsense, off-topic or fundamentally incorrect without fix → isValid=false

Task to validate:
Task Description: {draft.TaskDescription}
Code Solution: {draft.CodeSolution}
Code Generation Prompt: {draft.CodeGenerationPrompt ?? "EMPTY"}
Code Grading Prompt: {draft.CodeGradingPrompt}";
    }

    private static string? ExtractJson(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        raw = Regex.Replace(raw, "<think>[\\s\\S]*?</think>", "", RegexOptions.IgnoreCase);
        raw = raw
            .Replace("```", "")
            .Trim();

        int start = raw.IndexOf('{');
        int end = raw.LastIndexOf('}');
        if (start == -1 || end == -1 || end <= start)
            return null;

        return raw.Substring(start, end - start + 1);
    }
}
