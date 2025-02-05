namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IExcelProcessor
{
    Task<ExcelValidationResult> ProcessStudentsExcelFile(IFormFile file);
    void GenerateStudentRegistrationTemplate(Stream stream);
    ExcelValidationResult ProcessQuestionsExcelFile(IFormFile file);

}
