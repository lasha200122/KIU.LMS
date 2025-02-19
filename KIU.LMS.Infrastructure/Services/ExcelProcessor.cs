namespace KIU.LMS.Infrastructure.Services;

public class ExcelProcessor : IExcelProcessor
{
    public ExcelValidationResult ProcessQuestionsExcelFile(IFormFile file)
    {
        var result = new ExcelValidationResult();
        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    var rowNum = row.RowNumber();
                    try
                    {
                        var incorrectAnswers = new List<string>();
                        for (int i = 3; i <= 6; i++)
                        {
                            var answer = row.Cell(i).GetString().Trim();
                            if (!string.IsNullOrWhiteSpace(answer))
                                incorrectAnswers.Add(answer);
                        }

                        var question = new QuestionExcelDto(
                            row.Cell(1).GetString().Trim(),
                            row.Cell(2).GetString().Trim(),
                            incorrectAnswers
                        );

                        if (string.IsNullOrWhiteSpace(question.Question))
                        {
                            result.Errors.Add(new ExcelRowError(rowNum, "Question", "კითხვა სავალდებულოა"));
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(question.CorrectAnswer))
                        {
                            result.Errors.Add(new ExcelRowError(rowNum, "CorrectAnswer", "სწორი პასუხი სავალდებულოა"));
                            continue;
                        }

                        if (!question.IncorrectAnswers.Any())
                        {
                            result.Errors.Add(new ExcelRowError(rowNum, "IncorrectAnswers", "მინიმუმ ერთი არასწორი პასუხი სავალდებულოა"));
                            continue;
                        }

                        result.ValidQuestions.Add(question);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new ExcelRowError(rowNum, "General", $"მონაცემების წაკითხვის შეცდომა: {ex.Message}"));
                    }
                }
            }
        }
        result.IsValid = !result.Errors.Any();
        return result;
    }
    public async Task<ExcelValidationResult> ProcessStudentsExcelFile(IFormFile file)
    {
        var result = new ExcelValidationResult();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var rowNum = row.RowNumber();
                    try
                    {
                        var student = new StudentExcelDto(
                            row.Cell(1).GetString().Trim().ToLower(), 
                            row.Cell(2).GetString().Trim().ToLower(),
                            row.Cell(3).GetString().Trim().ToLower());

                        if (string.IsNullOrWhiteSpace(student.FirstName))
                        {
                            result.Errors.Add(new ExcelRowError(
                                rowNum,
                                "FirstName",
                                "სახელი სავალდებულოა"));

                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(student.LastName))
                        {
                            result.Errors.Add(new ExcelRowError(
                                rowNum,
                                "LastName",
                                "გვარი სავალდებულოა"));

                            continue;
                        }

                        if (!IsValidEmail(student.Email))
                        {
                            result.Errors.Add(new ExcelRowError(
                                rowNum,
                                "Email",
                                "არასწორი ელ.ფოსტის ფორმატი"));

                            continue;
                        }

                        result.ValidStudents.Add(student);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new ExcelRowError(
                            rowNum,
                            "General",
                           $"მონაცემების წაკითხვის შეცდომა: {ex.Message}"));
                    }
                }
            }
        }

        result.IsValid = !result.Errors.Any();
        return result;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public void GenerateStudentRegistrationTemplate(Stream stream)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Students");

            worksheet.Cell(1, 1).Value = "სახელი";
            worksheet.Cell(1, 2).Value = "გვარი";
            worksheet.Cell(1, 3).Value = "ელ.ფოსტა";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(stream);
        }
    }

    public void GenerateQuestionsTemplate(Stream stream)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Questions");

            worksheet.Cell(1, 1).Value = "კითხვა";
            worksheet.Cell(1, 2).Value = "სწორი პასუხი";
            worksheet.Cell(1, 3).Value = "არასწორი პასუხი 1";
            worksheet.Cell(1, 4).Value = "არასწორი პასუხი 2";
            worksheet.Cell(1, 5).Value = "არასწორი პასუხი 3";
            worksheet.Cell(1, 6).Value = "არასწორი პასუხი 4";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(stream);
        }
    }

    public void GenerateEmailListTemplate(Stream stream)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Emails");

            worksheet.Cell(1, 1).Value = "ელ.ფოსტა";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(stream);
        }
    }

    public async Task<List<string>> ProcessEmailListFile(IFormFile file)
    {
        var emails = new List<string>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    try
                    {
                        var email = row.Cell(1).GetString().Trim().ToLower();

                        if (IsValidEmail(email))
                        {
                            emails.Add(email);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        return emails;
    }
}