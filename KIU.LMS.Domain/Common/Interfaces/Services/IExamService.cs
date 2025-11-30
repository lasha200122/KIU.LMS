using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IExamService
{
    Task<ExamSession> StartExamAsync(Guid studentId, Guid quizId);
    Task<Domain.Entities.NoSQL.ExamQuestion?> GetCurrentQuestionAsync(string sessionId);
    Task<bool> SubmitAnswerAsync(
        string sessionId,
        string questionId,
        List<string>? selectedOptions = null,
        string? studentCode = null,
        string? studentPrompt = null);
    Task<ExamSession?> GetSessionByIdAsync(string sessionId);
    Task<bool> PauseExamAsync(string sessionId);
    Task<bool> ResumeExamAsync(string sessionId);
    Task<bool> FinishExamAsync(string sessionId);
    Task<List<string>> GetStudentAnswer(string sessionId, string questionId);
}