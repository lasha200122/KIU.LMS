using KIU.LMS.Domain.Common.Enums.Assignment;

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
                        for (int i = 3; i <= 5; i++)
                        {
                            var answer = row.Cell(i).GetString().Trim();
                            if (!string.IsNullOrWhiteSpace(answer))
                                incorrectAnswers.Add(answer);
                        }
                        
                        var question = new QuestionExcelDto(
                            row.Cell(1).GetString().Trim(),
                            row.Cell(2).GetString().Trim(),
                            incorrectAnswers,
                            row.Cell(6).GetString().Trim(),
                            row.Cell(7).GetString().Trim()
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

                        if (string.IsNullOrWhiteSpace(question.ExplanationCorrectAnswer))
                        {
                            result.Errors.Add(new ExcelRowError(rowNum, "ExplanationCorrectAnswer", "სწორი პასუხის ახსნა სავალდებულოა"));
                            continue;
                        }
                        
                        if (string.IsNullOrWhiteSpace(question.ExplanationIncorrectAnswer))
                        {
                            result.Errors.Add(new ExcelRowError(rowNum, "ExplanationIncorrectAnswer", "არასწორი პასუხის ახსნა სავალდებულოა"));
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
                            row.Cell(3).GetString().Trim().ToLower(),
                            row.Cell(4).GetString().Trim());

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
            worksheet.Cell(1, 6).Value = "სწორი პასუხის ახსნა";
            worksheet.Cell(1, 7).Value = "არასწორი პასუხის ახსნა";

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

    public void GenerateQuizResults(Stream stream, IEnumerable<QuizResultDto> quizResults)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Quiz Results");
            worksheet.Cell(1, 1).Value = "ადგილი";
            worksheet.Cell(1, 2).Value = "სახელი";
            worksheet.Cell(1, 3).Value = "გვარი";
            worksheet.Cell(1, 4).Value = "ელ.ფოსტა";
            worksheet.Cell(1, 5).Value = "სკოლა";
            worksheet.Cell(1, 6).Value = "დაწყების დრო";
            worksheet.Cell(1, 7).Value = "დასრულების დრო";
            worksheet.Cell(1, 8).Value = "ქულა";
            worksheet.Cell(1, 9).Value = "პროცენტი";
            worksheet.Cell(1, 10).Value = "კითხვების რაოდენობა";
            worksheet.Cell(1, 11).Value = "სწორი პასუხები";
            worksheet.Cell(1, 12).Value = "არასწორი პასუხები";
            worksheet.Cell(1, 13).Value = "ხანგრძლივობა";
            worksheet.Cell(1, 14).Value = "უარყოფითი ქულა";
            worksheet.Cell(1, 15).Value = "ბონუს ქულა";
            worksheet.Cell(1, 16).Value = "საბოლოო ქულა";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var result in quizResults)
            {
                worksheet.Cell(row, 1).Value = result.Rank;
                worksheet.Cell(row, 2).Value = result.FirstName;
                worksheet.Cell(row, 3).Value = result.LastName;
                worksheet.Cell(row, 4).Value = result.Email;
                worksheet.Cell(row, 5).Value = result.Institution;
                worksheet.Cell(row, 6).Value = result.StartedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                worksheet.Cell(row, 7).Value = result.FinishedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                worksheet.Cell(row, 8).Value = result.Score;
                worksheet.Cell(row, 9).Value = result.Percentage;
                worksheet.Cell(row, 10).Value = result.TotalQuestions;
                worksheet.Cell(row, 11).Value = result.CorrectAnswers;
                worksheet.Cell(row, 12).Value = result.WrongAnswers;

                var duration = result.Duration;
                worksheet.Cell(row, 13).Value = $"{(int)duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}";

                worksheet.Cell(row, 14).Value = result.MinusPoint;
                worksheet.Cell(row, 15).Value = result.Bonus;
                worksheet.Cell(row, 16).Value = result.FinalScore;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            var dataRange = worksheet.Range(2, 1, row - 1, 10);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Column(6).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
            worksheet.Column(7).Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";

            worksheet.Column(8).Style.NumberFormat.Format = "0.00";
            worksheet.Column(9).Style.NumberFormat.Format = "0.00";
            worksheet.Column(15).Style.NumberFormat.Format = "0.00";
            worksheet.Column(16).Style.NumberFormat.Format = "0.00";

            if (quizResults.Any(r => r.MinusPoint.HasValue))
            {
                worksheet.Column(14).Style.NumberFormat.Format = "0.00";
            }

            workbook.SaveAs(stream);
        }
    }

    public void GenerateFinalists(Stream stream, IEnumerable<SchoolRankingItemFinal> quizResults)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Final Results");
            worksheet.Cell(1, 1).Value = "ადგილი";
            worksheet.Cell(1, 2).Value = "მონაწილე";
            worksheet.Cell(1, 3).Value = "ელ.ფოსტა";
            worksheet.Cell(1, 4).Value = "ქულა";

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var result in quizResults)
            {
                worksheet.Cell(row, 1).Value = result.Rank;
                worksheet.Cell(row, 2).Value = result.Name;
                worksheet.Cell(row, 3).Value = result.Email;
                worksheet.Cell(row, 4).Value = result.Value;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var dataRange = worksheet.Range(2, 1, row - 1, 10);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Column(4).Style.NumberFormat.Format = "0.00";

            workbook.SaveAs(stream);
        }
    }


    public void GenerateC2RSTemplate(Stream stream)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Tasks");

            // Set headers
            worksheet.Cell(1, 1).Value = "Task Description";
            worksheet.Cell(1, 2).Value = "Code Solution";
            worksheet.Cell(1, 3).Value = "Code Generation Prompt";
            worksheet.Cell(1, 4).Value = "Code Grading Prompt";
            worksheet.Cell(1, 5).Value = "Difficulty";

            // Style the header row
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Set column widths for better readability
            worksheet.Column(1).Width = 40; // Task Description
            worksheet.Column(2).Width = 50; // Code Solution
            worksheet.Column(3).Width = 45; // Code Generation Prompt
            worksheet.Column(4).Width = 45; // Code Grading Prompt
            worksheet.Column(5).Width = 15; // Difficulty

            // Apply text wrapping to all columns except Difficulty
            worksheet.Column(1).Style.Alignment.WrapText = true;
            worksheet.Column(2).Style.Alignment.WrapText = true;
            worksheet.Column(3).Style.Alignment.WrapText = true;
            worksheet.Column(4).Style.Alignment.WrapText = true;

            // Add sample row to demonstrate the template
            worksheet.Cell(2, 1).Value = "Create a function called calculate_factorial_recursive that calculates the factorial of a number using recursion (5! = 5×4×3×2×1 = 120)";
            worksheet.Cell(2, 2).Value = @"python<br>def calculate_factorial_recursive(n: int) -> int:<br>if n <= 1:<br>return 1<br>return n * calculate_factorial_recursive(n - 1)";
            worksheet.Cell(2, 3).Value = "Write a Python function called 'calculate_factorial_recursive' that calculates factorial using recursion. Include proper base case for n <= 1 and recursive case. Use proper type annotations.";
            worksheet.Cell(2, 4).Value = "Evaluate this calculate_factorial_recursive function: Does it correctly implement recursion with proper base case? Does it calculate factorial correctly? Are type annotations appropriate?";
            worksheet.Cell(2, 5).Value = "Easy";

            // Style the sample row
            worksheet.Range(2, 1, 2, 5).Style.Fill.BackgroundColor = XLColor.LightBlue;
            worksheet.Range(2, 1, 2, 5).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            // Apply borders to the header and sample row
            var dataRange = worksheet.Range(1, 1, 2, 5);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Adjust row heights
            worksheet.Row(1).Height = 30;
            worksheet.Row(2).Height = 100;

            workbook.SaveAs(stream);
        }
    }

    public void GetGeneratedAssigmentQuestions(Stream stream, IEnumerable<GeneratedQuestion> questions)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Questions");

        worksheet.Cell(1, 1).Value = "კითხვა";
        worksheet.Cell(1, 2).Value = "სწორი პასუხი";
        worksheet.Cell(1, 3).Value = "არასწორი პასუხი 1";
        worksheet.Cell(1, 4).Value = "არასწორი პასუხი 2";
        worksheet.Cell(1, 5).Value = "არასწორი პასუხი 3";
        worksheet.Cell(1, 6).Value = "სწორი პასუხის ახსნა";
        worksheet.Cell(1, 7).Value = "არასწორი პასუხის ახსნა";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var question in questions)
        {
            worksheet.Cell(row, 1).Value = question.QuestionText;
            worksheet.Cell(row, 2).Value = question.OptionA; // Always correct
            worksheet.Cell(row, 3).Value = question.OptionB;
            worksheet.Cell(row, 4).Value = question.OptionC;
            worksheet.Cell(row, 5).Value = question.OptionD;
            worksheet.Cell(row, 6).Value = question.ExplanationCorrect;
            worksheet.Cell(row, 7).Value = question.ExplanationIncorrect;

            row++;
        }

        worksheet.Columns().AdjustToContents();

        // Apply borders
        if (row > 2)
        {
            var dataRange = worksheet.Range(1, 1, row - 1, 7);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        workbook.SaveAs(stream);
        stream.Position = 0;
    }

    public void GetGeneratedAssignmentTasks(
        Stream stream, IEnumerable<GeneratedTask> tasks,
        GeneratedAssignmentType type, DifficultyType difficulty)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Tasks");

        worksheet.Cell(1, 1).Value = "Task Description";
        worksheet.Cell(1, 2).Value = "Code Solution";
        worksheet.Cell(1, 3).Value = "Code Generation Prompt";
        worksheet.Cell(1, 4).Value = "Code Grading Prompt";
        worksheet.Cell(1, 5).Value = "Difficulty";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;
        foreach (var task in tasks)
        {
            worksheet.Cell(row, 1).Value = task.TaskDescription;
            worksheet.Cell(row, 2).Value = task.CodeSolution;
        
            if (type == GeneratedAssignmentType.C2RS)
            {
                worksheet.Cell(row, 3).Value = "EMPTY";
            }
            else  
            {
                worksheet.Cell(row, 3).Value = string.IsNullOrWhiteSpace(task.CodeGenerationPrompt) 
                    ? "EMPTY" 
                    : task.CodeGenerationPrompt;
            }
        
            worksheet.Cell(row, 4).Value = task.CodeGradingPrompt;
            worksheet.Cell(row, 5).Value = difficulty.ToString();

            row++;
        }

        worksheet.Columns().AdjustToContents();

        // Apply borders
        if (row > 2)
        {
            var dataRange = worksheet.Range(1, 1, row - 1, 5);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        workbook.SaveAs(stream);
        stream.Position = 0;
    }

    
    public async Task<ExcelValidationResult> ProcessTasksExcelFile(IFormFile file)
    {
        var result = new ExcelValidationResult();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1); // Skip header row

                foreach (var row in rows)
                {
                    var rowNum = row.RowNumber();
                    try
                    {
                        var task = new C2RSExcelDto(
                            row.Cell(1).GetString().Trim(), // Task Description
                            row.Cell(2).GetString().Trim(), // Code Solution
                            row.Cell(3).GetString().Trim(), // Code Generation Prompt
                            row.Cell(4).GetString().Trim(), // Code Grading Prompt
                            row.Cell(5).GetString().Trim()  // Difficulty
                        );

                        // Validate Task Description
                        if (string.IsNullOrWhiteSpace(task.TaskDescription))
                        {
                            result.Errors.Add(new ExcelRowError(
                                rowNum,
                                "TaskDescription",
                                "დავალების აღწერა სავალდებულოა"));
                            continue;
                        }

                        // Validate Code Solution
                        if (string.IsNullOrWhiteSpace(task.CodeSolution))
                        {
                            result.Errors.Add(new ExcelRowError(
                                rowNum,
                                "CodeSolution",
                                "კოდის გადაწყვეტა სავალდებულოა"));
                            continue;
                        }

                        // Validate Code Generation Prompt
                        if (string.IsNullOrWhiteSpace(task.CodeGenerationPrompt))
                        {
                            result.Errors.Add(new ExcelRowError(
                                rowNum,
                                "CodeGenerationPrompt",
                                "კოდის გენერაციის პრომპტი სავალდებულოა"));
                            continue;
                        }

                        // Validate Code Grading Prompt
                        if (string.IsNullOrWhiteSpace(task.CodeGradingPrompt))
                        {
                            result.Errors.Add(new ExcelRowError(
                                rowNum,
                                "CodeGradingPrompt",
                                "კოდის შეფასების პრომპტი სავალდებულოა"));
                            continue;
                        }

                        // Validate Difficulty
                        if (!IsValidDifficulty(task.Difficulty))
                        {
                            result.Errors.Add(new ExcelRowError(
                                rowNum,
                                "Difficulty",
                                "სირთულე უნდა იყოს: Easy, Medium ან Hard"));
                            continue;
                        }

                        result.ValidTasks.Add(task);
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


    public static DifficultyType ParseDifficulty(string difficulty)
    {
        return difficulty.ToLower() switch
        {
            "easy" => DifficultyType.Easy,
            "medium" => DifficultyType.Medium,
            "hard" => DifficultyType.Hard,
            _ => DifficultyType.None
        };
    }

    private bool IsValidDifficulty(string difficulty)
    {
        if (string.IsNullOrWhiteSpace(difficulty))
            return false;

        var validDifficulties = new[] { "Easy", "Medium", "Hard" };
        return validDifficulties.Contains(difficulty, StringComparer.OrdinalIgnoreCase);
    }
}