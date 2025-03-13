using Anthropic.SDK.Messaging;
using KIU.LMS.Domain.Common.Models.Gemini;
using System.Text.Json;

namespace KIU.LMS.Infrastructure.Services;

public class GradingService(IClaudeService _gemini) : IGradingService
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

        try
        {
            var claudeRequest = BuildGradingPrompt(solution.Assignment, solution);
            var response = await _gemini.GenerateContentAsync(claudeRequest.Messages, claudeRequest.SystemMessages);

            if (response == null)
                return (false, string.Empty, string.Empty);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            var gradingResult = JsonSerializer.Deserialize<GradingResult>(response, options);

            if (gradingResult != null)
            {
                var combinedFeedback = CombineFeedback(gradingResult.feedback, gradingResult.suggestions);
                return (true, gradingResult.grade.ToString(), combinedFeedback);
            }

            await Task.Delay(10000);
        }
        catch (Exception ex)
        {
            await Task.Delay(1000);
            return (false, string.Empty, string.Empty);
        }

        return (false, string.Empty, string.Empty);
    }

    private (List<Message> Messages, List<SystemMessage> SystemMessages) BuildGradingPrompt(Assignment assignment, Solution solution)
    {
        string score = assignment.Score.HasValue ? assignment.Score.Value.ToString() : "does not have grade";

        var systemMessages = new List<SystemMessage>
    {
        new("""
            You are an expert code evaluator and grading assistant with deep knowledge in software development.
            Your task is to evaluate student code submissions with the following strict requirements:
            """),
        new("""
            Response Format:
            {
                "grade": numeric (0-[maxScore]),
                "feedback": constructive evaluation (300 chars max),
                "suggestions": specific improvements (200 chars max)
            }
            """),
        new("""
            Grading Guidelines:
            - Evaluate against industry best practices
            - Consider code quality, efficiency, and readability
            - Check for proper error handling and edge cases
            - Assess code organization and maintainability
            - Look for security considerations
            - Verify proper documentation and comments
            """),
        new("""
            Response Rules:
            - Return ONLY valid JSON format
            - Stay within character limits
            - Be specific and actionable in feedback
            - Provide practical improvement suggestions
            - No conversation or additional text
            - Grade strictly - full score only for exceptional work
            """)
    };

        var messages = new List<Message>
    {
        new Message(RoleType.User, $"""
            Additional Prompt:
            {assignment.Prompt.Value ?? "No specific promp provided"}
            """),

        new Message(RoleType.User, $"""
            Assignment Details:
            Original Task:
            {assignment.Problem ?? "No specific problem description provided"}
            """),

        new Message(RoleType.User, $"""
            Reference Implementation:
            {assignment.Code ?? "No reference implementation provided"}
            """),

        new Message(RoleType.User, $"""
            Grading Information:
            Maximum Score: {score}
            """),

        new Message(RoleType.User, $"""
            Student Submission:
            {solution.Value}
            
            Evaluate the above submission according to the specified criteria and format.
            """)
    };

        return new(messages, systemMessages);
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
