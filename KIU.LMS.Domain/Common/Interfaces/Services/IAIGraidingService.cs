namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IAiGradingService
{
    Task<string?> GradeAsync(string problem, string referenceSolution, string studentSubmission, string rubric);
    Task<(bool isValid, string? reason)> ValidateAsync(string problem, string referenceSolution, string studentSubmission, string rubric, string gradeResultJson);
}
