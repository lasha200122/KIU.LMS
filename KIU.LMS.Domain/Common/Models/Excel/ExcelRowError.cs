namespace KIU.LMS.Domain.Common.Models.Excel;

public sealed record ExcelRowError(int RowNumber, string ColumnName, string ErrorMessage);