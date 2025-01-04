namespace KIU.LMS.Domain.Common.Models.Excel;

public class ExcelValidationResult
{
    public bool IsValid { get; set; }
    public List<ExcelRowError> Errors { get; set; } = new();
    public List<StudentExcelDto> ValidStudents { get; set; } = new();
}