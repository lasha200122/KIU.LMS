namespace KIU.LMS.Domain.Common.Models.Excel;

public record C2RSExcelDto(
    string TaskDescription,
    string CodeSolution,
    string CodeGenerationPrompt,
    string CodeGradingPrompt,
    string Difficulty
);