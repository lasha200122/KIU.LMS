using KIU.LMS.Domain.Common.Models.Gemini;
using System.Text.Json;

namespace KIU.LMS.Infrastructure.Services;

public class GradingService(IGeminiService _gemini) : IGradingService
{
    public async Task<(bool success, string message)> GradeSubmissionAsync(Solution solution)
    {
        try
        {
            var (success, grade, feedback) = await ProcessGradingWithRetryAsync(solution);

            if (!success)
            {
                solution.Failed();
                return (false, "Grading failed after multiple attempts");
            }
            solution.Graded(grade, feedback);

            return (true, "Grading completed successfully");
        }
        catch (Exception ex)
        {
            return (false, "Unexpected error occurred during grading");
        }
    }

    private async Task<(bool success, string grade, string feedback)> ProcessGradingWithRetryAsync(Solution solution)
    {
        const int maxRetries = 10;
        var attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                var prompt = BuildGradingPrompt(solution.Assignment, solution);
                var response = await _gemini.GenerateContentAsync(prompt);

                if (string.IsNullOrEmpty(response))
                {
                    attempts++;
                    continue;
                }

                var gradingResult = ParseAndValidateResponse(response, solution.Assignment.Score);
                if (gradingResult != null)
                {
                    var combinedFeedback = CombineFeedback(gradingResult.GetFormattedFeedback(), gradingResult.GetFormattedSuggestions());
                    return (true, gradingResult.grade.ToString(), combinedFeedback);
                }

                attempts++;
                await Task.Delay(1000 * attempts);
            }
            catch (Exception ex)
            {
                attempts++;

                if (attempts == maxRetries)
                    return (false, string.Empty, string.Empty);

                await Task.Delay(1000 * attempts);
            }
        }

        return (false, string.Empty, string.Empty);
    }

    private string BuildGradingPrompt(Assignment assignment, Solution solution)
    {
        string score = assignment.Score.HasValue ? assignment.Score.Value.ToString() : "does not have grade";
        return $@" {assignment.Prompt.Value}

Assignment Context:
{assignment.Problem?? string.Empty}
{assignment.Code?? string.Empty}
Maximum Possible Score: {score}

Student Submission for Evaluation:
{solution.Value}

Provide your detailed evaluation in the following JSON format only:
{{
    ""grade"": ""numeric value between 0-{score}"",
    ""feedback"": ""detailed evaluation of the solution (max 300 characters)"",
    ""suggestions"": ""specific recommendations for improvement (max 200 characters)""
}}

Important:
- Keep feedback and suggestions concise and to the point
- Do not exceed the character limits specified above
- Maintain high academic standards in your evaluation
- Do not award full points unless the solution demonstrates exceptional quality
- Ensure your response contains only this JSON object with no additional text
- Focus on the most important aspects in your limited feedback space";
    }

    private GradingResult ParseAndValidateResponse(string response, decimal? maxScore)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            var cleanedResponse = CleanJsonResponse(response);
            var gradingResult = JsonSerializer.Deserialize<GradingResult>(cleanedResponse, options);

            if (gradingResult == null ||
                string.IsNullOrEmpty(gradingResult.grade.ToString()) ||
                string.IsNullOrEmpty(gradingResult.feedback))
            {
                return null;
            }

            if (!ValidateGrade(gradingResult.grade.ToString(), maxScore))
            {
                return null;
            }

            return gradingResult;
        }
        catch (JsonException ex)
        {
            return null;
        }
    }

    private string CleanJsonResponse(string response)
    {
        var jsonStart = response.IndexOf("{");
        var jsonEnd = response.LastIndexOf("}");

        if (jsonStart == -1 || jsonEnd == -1)
            return response;

        return response.Substring(jsonStart, jsonEnd - jsonStart + 1)
                      .Replace("\r", "");
    }

    private bool ValidateGrade(string grade, decimal? maxScore)
    {
        if (!decimal.TryParse(grade, out decimal numericGrade))
            return false;

        return numericGrade >= 0 && maxScore.HasValue && numericGrade <= maxScore.Value;
    }

    private string CombineFeedback(string feedback, string suggestions)
    {
        var combinedFeedback = new StringBuilder();
        combinedFeedback.AppendLine("Evaluation:");
        combinedFeedback.AppendLine(feedback);

        if (!string.IsNullOrEmpty(suggestions))
        {
            combinedFeedback.AppendLine();
            combinedFeedback.AppendLine("Suggestions for Improvement:");
            combinedFeedback.AppendLine(suggestions);
        }

        return combinedFeedback.ToString().Trim();
    }
}
