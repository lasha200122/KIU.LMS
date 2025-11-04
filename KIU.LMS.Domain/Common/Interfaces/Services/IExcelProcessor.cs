namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IExcelProcessor
{
    Task<ExcelValidationResult> ProcessStudentsExcelFile(IFormFile file);
    ExcelValidationResult ProcessQuestionsExcelFile(IFormFile file);

    void GenerateStudentRegistrationTemplate(Stream stream);
    void GenerateQuestionsTemplate(Stream stream);
    void GenerateEmailListTemplate(Stream stream); 
    Task<List<string>> ProcessEmailListFile(IFormFile file);
    void GenerateQuizResults(Stream stream, IEnumerable<QuizResultDto> quizResults);
    void GenerateFinalists(Stream stream, IEnumerable<SchoolRankingItemFinal> quizResults);
    void GenerateC2RSTemplate(Stream stream);
    public void GetGeneratedAssigmentQuestions(Stream stream,
        IEnumerable<GeneratedQuestion> questions);
    Task<ExcelValidationResult> ProcessTasksExcelFile(IFormFile file);
}


public sealed record SchoolRankingItemFinal(
    int Rank,
    string Name,
    string Value,
    string Email);