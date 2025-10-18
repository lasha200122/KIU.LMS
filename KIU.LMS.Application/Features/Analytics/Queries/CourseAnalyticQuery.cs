namespace KIU.LMS.Application.Features.Analytics.Queries;

using Domain.Entities.NoSQL;
using Microsoft.Extensions.Logging;

public sealed record CourseAnalyticsQuery(Guid CourseId) : IRequest<Result<CourseFullAnalyticsDto>>;

public sealed class CourseAnalyticsQueryHandler(
    IUnitOfWork _unitOfWork,
    IMongoRepository<ExamSession> _examSessionRepository,
    IMongoRepository<StudentAnswer> _studentAnswerRepository,
    ILogger<CourseAnalyticsQueryHandler> _logger) 
    : IRequestHandler<CourseAnalyticsQuery, Result<CourseFullAnalyticsDto>>
{
    public async Task<Result<CourseFullAnalyticsDto>> Handle(
        CourseAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await GetCourseAnalyticsAsync(request.CourseId, cancellationToken);
            return dto != null
                ? Result<CourseFullAnalyticsDto>.Success(dto)
                : Result<CourseFullAnalyticsDto>.Failure("Course not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course analytics for {CourseId}", request.CourseId);
            return Result<CourseFullAnalyticsDto>.Failure(
                $"An error occurred while fetching course analytics: {ex.Message}");
        }
    }

    private async Task<CourseFullAnalyticsDto> GetCourseAnalyticsAsync(
        Guid courseId, CancellationToken ct)
    {
        var courseExists = await _unitOfWork.CourseRepository
            .ExistsAsync(c => c.Id == courseId, ct);
        
        if (!courseExists)
        {
            _logger.LogWarning("Course {CourseId} not found", courseId);
            return null;
        }

        var courseInfo = await GetCourseInfoAsync(courseId, ct);
        
        var students = await _unitOfWork.UserCourseRepository
            .GetMappedAsync(
                uc => uc.CourseId == courseId && uc.User.Role == UserRole.Student,
                uc => new StudentBasicInfo
                {
                    Id = uc.UserId,
                    Name = uc.User.FirstName + " " + uc.User.LastName,
                    Email = uc.User.Email ?? ""
                },
                ct);

        if (!students.Any())
        {
            return CreateEmptyAnalytics(courseInfo);
        }

        var studentIds = students.Select(s => s.Id).ToList();

        var solutions = await _unitOfWork.SolutionRepository
            .GetMappedAsync(
                s => studentIds.Contains(s.UserId) && s.Assignment.CourseId == courseId,
                s => new SolutionData
                {
                    UserId = s.UserId,
                    Grade = s.Grade,
                    IsTraining = s.Assignment.IsTraining,
                    CreateDate = s.CreateDate,
                    AssignmentType = s.Assignment.Type.ToString()
                },
                ct);

        var examResults = await _unitOfWork.ExamResultRepository
            .GetMappedAsync(
                er => studentIds.Contains(er.StudentId) && er.Quiz.CourseId == courseId,
                er => new ExamResultData
                {
                    StudentId = er.StudentId,
                    Score = er.Score,
                    FinishedAt = er.FinishedAt,
                    Duration = er.Duration.TotalSeconds
                },
                ct);

        var quizIds = (await _unitOfWork.QuizRepository
            .GetMappedAsync(
                q => q.CourseId == courseId,
                q => q.Id.ToString(),
                ct)).ToList();

        var mongoData = await GetMongoDataAsync(quizIds, ct);

        var processedData = ProcessStudentData(
            students.ToList(), 
            solutions.ToList(), 
            examResults.ToList());

        var kpis = BuildKPIs(processedData, courseInfo.TotalAssignments);
        var engagement = BuildEngagement(processedData, students.Count, mongoData);
        var performance = BuildPerformance(processedData);
        var trends = BuildTrends(processedData);
        var studentPerformance = BuildStudentPerformanceList(
            processedData, 
            courseInfo.TotalAssignments);
        var atRisk = BuildAtRiskStudents(processedData, courseInfo.TotalAssignments);

        return new CourseFullAnalyticsDto
        {
            CourseInfo = courseInfo,
            Kpis = kpis,
            Engagement = engagement,
            Performance = performance,
            Trends = trends,
            StudentPerformance = studentPerformance,
            AtRiskStudents = atRisk,
            ResourceUsage = new ResourceUsageSection 
            { 
                TotalResourcesAccessed = 0,
                AverageTimeSpent = 0,
                MostAccessedResources = new List<ResourceAccessItem>()
            },
            GeneratedAt = DateTimeOffset.UtcNow
        };
    }
    
    private async Task<CourseInfoSection> GetCourseInfoAsync(Guid courseId, CancellationToken ct)
    {
        var enrollmentsCount = await _unitOfWork.UserCourseRepository
            .CountWithPredicateAsync(
                uc => uc.CourseId == courseId && uc.User.Role == UserRole.Student, 
                ct);
        
        var assignmentsCount = await _unitOfWork.AssignmentRepository
            .CountWithPredicateAsync(a => a.CourseId == courseId, ct);
        
        var quizzesCount = await _unitOfWork.QuizRepository
            .CountWithPredicateAsync(q => q.CourseId == courseId, ct);
        
        var topicsCount = await _unitOfWork.TopicRepository
            .CountWithPredicateAsync(t => t.CourseId == courseId, ct);
        
        var modulesCount = await _unitOfWork.ModuleRepository
            .CountWithPredicateAsync(m => m.CourseId == courseId, ct);

        return new CourseInfoSection
        {
            TotalStudents = enrollmentsCount,
            TotalAssignments = assignmentsCount,
            TotalQuizzes = quizzesCount,
            TotalTopics = topicsCount,
            TotalModules = modulesCount
        };
    }

    private async Task<MongoDataSummary> GetMongoDataAsync(
        List<string> quizIds, 
        CancellationToken ct)
    {
        if (!quizIds.Any())
            return new MongoDataSummary();

        var sessions = await _examSessionRepository
            .FindAsync(es => quizIds.Contains(es.QuizId));
        var sessionsList = sessions?.ToList() ?? new List<ExamSession>();
        
        var sessionIds = sessionsList.Select(s => s.Id).ToList();
        var answers = sessionIds.Any()
            ? await _studentAnswerRepository.FindAsync(sa => sessionIds.Contains(sa.SessionId))
            : null;

        return new MongoDataSummary
        {
            TotalSessions = sessionsList.Count,
            CompletedSessions = sessionsList.Count(s => s.Status.ToString() == "Completed"),
            TotalAnswers = answers?.Count() ?? 0
        };
    }

    private List<StudentProcessedData> ProcessStudentData(
        List<StudentBasicInfo> students,
        List<SolutionData> solutions,
        List<ExamResultData> examResults)
    {
        return students.Select(student =>
        {
            var studentSolutions = solutions.Where(s => s.UserId == student.Id).ToList();
            var studentExams = examResults.Where(e => e.StudentId == student.Id).ToList();

            var trainingSolutions = studentSolutions.Where(s => s.IsTraining).ToList();
            var gradingSolutions = studentSolutions.Where(s => !s.IsTraining).ToList();

            var trainingGrades = ParseGrades(trainingSolutions.Select(s => s.Grade).ToList());
            var gradingGrades = ParseGrades(gradingSolutions.Select(s => s.Grade).ToList());
            var quizScores = studentExams.Select(e => e.Score).ToList();

            var lastSolutionDate = studentSolutions.Any() 
                ? studentSolutions.Max(s => s.CreateDate) 
                : DateTimeOffset.MinValue;
            var lastExamDate = studentExams.Any() 
                ? studentExams.Max(e => e.FinishedAt) 
                : DateTimeOffset.MinValue;

            return new StudentProcessedData
            {
                Student = student,
                TotalSolutions = studentSolutions.Count,
                TrainingSolutions = trainingSolutions.Count,
                GradingSolutions = gradingSolutions.Count,
                QuizCount = studentExams.Count,
                TrainingGrades = trainingGrades,
                GradingGrades = gradingGrades,
                QuizScores = quizScores,
                LastActivity = lastSolutionDate > lastExamDate ? lastSolutionDate : lastExamDate,
                AllActivities = studentSolutions.Select(s => new ActivityData 
                    { 
                        Date = s.CreateDate, 
                        Grade = ParseSingleGrade(s.Grade) 
                    })
                    .Concat(studentExams.Select(e => new ActivityData 
                    { 
                        Date = e.FinishedAt, 
                        Grade = e.Score 
                    }))
                    .ToList()
            };
        }).ToList();
    }

    private KPIsSection BuildKPIs(
        List<StudentProcessedData> studentsData, 
        int totalAssignments)
    {
        var allGrades = studentsData
            .SelectMany(s => s.TrainingGrades.Concat(s.GradingGrades).Concat(s.QuizScores))
            .ToList();

        var totalSubmissions = studentsData.Sum(s => s.TotalSolutions + s.QuizCount);
        var totalPossibleSubmissions = studentsData.Count * totalAssignments;

        return new KPIsSection
        {
            AverageGrade = allGrades.Any() ? Math.Round(allGrades.Average(), 2) : 0,
            PassRate = allGrades.Any() 
                ? Math.Round((decimal)allGrades.Count(g => g >= 60) / allGrades.Count * 100, 2) 
                : 0,
            CompletionRate = totalPossibleSubmissions > 0
                ? Math.Round((decimal)totalSubmissions / totalPossibleSubmissions * 100, 2)
                : 0,
            TotalSubmissions = totalSubmissions,
            EngagementScore = studentsData.Any()
                ? Math.Round((decimal)totalSubmissions / studentsData.Count, 2)
                : 0
        };
    }

    private EngagementSection BuildEngagement(
        List<StudentProcessedData> studentsData,
        int totalStudents,
        MongoDataSummary mongoData)
    {
        var totalAttempts = studentsData.Sum(s => s.TotalSolutions + s.QuizCount);
        var activeStudents = studentsData.Count(s => s.TotalSolutions > 0 || s.QuizCount > 0);
        var activePercentage = totalStudents > 0
            ? Math.Round((decimal)activeStudents / totalStudents * 100, 2)
            : 0;

        return new EngagementSection
        {
            AverageAttemptsPerStudent = totalStudents > 0
                ? Math.Round((decimal)totalAttempts / totalStudents, 2)
                : 0,
            ActiveStudentsPercentage = activePercentage,
            DropOffRate = Math.Round(100 - activePercentage, 2),
            AverageTimePerAssessment = 0,
            TotalInteractions = totalAttempts + mongoData.TotalAnswers,
            ActivityByType = new Dictionary<string, int>
            {
                ["Training"] = studentsData.Sum(s => s.TrainingSolutions),
                ["Assignments"] = studentsData.Sum(s => s.GradingSolutions),
                ["Quizzes"] = studentsData.Sum(s => s.QuizCount),
                ["Exam Sessions"] = mongoData.CompletedSessions
            }
        };
    }

    private PerformanceSection BuildPerformance(List<StudentProcessedData> studentsData)
    {
        var allTrainingGrades = studentsData.SelectMany(s => s.TrainingGrades).ToList();
        var allGradingGrades = studentsData
            .SelectMany(s => s.GradingGrades.Concat(s.QuizScores))
            .ToList();

        var trainingAvg = allTrainingGrades.Any() 
            ? Math.Round(allTrainingGrades.Average(), 2) : 0;
        var gradingAvg = allGradingGrades.Any() 
            ? Math.Round(allGradingGrades.Average(), 2) : 0;

        return new PerformanceSection
        {
            TrainingMode = new TrainingModeStats
            {
                AverageScore = trainingAvg,
                AverageImprovement = 0,
                TotalAttempts = studentsData.Sum(s => s.TrainingSolutions),
                PassRate = allTrainingGrades.Any()
                    ? Math.Round((decimal)allTrainingGrades.Count(g => g >= 60) / allTrainingGrades.Count * 100, 2)
                    : 0
            },
            GradingMode = new GradingModeStats
            {
                AverageScore = gradingAvg,
                TotalSubmissions = studentsData.Sum(s => s.GradingSolutions + s.QuizCount),
                PassRate = allGradingGrades.Any()
                    ? Math.Round((decimal)allGradingGrades.Count(g => g >= 60) / allGradingGrades.Count * 100, 2)
                    : 0,
                ScoresByType = new Dictionary<string, decimal>()
            },
            Comparison = new ComparisonStats
            {
                TrainingAverage = trainingAvg,
                TrainingCount = studentsData.Sum(s => s.TrainingSolutions),
                GradingAverage = gradingAvg,
                GradingCount = studentsData.Sum(s => s.GradingSolutions + s.QuizCount),
                ImprovementRate = trainingAvg > 0 && gradingAvg > 0
                    ? Math.Round(((gradingAvg - trainingAvg) / trainingAvg) * 100, 2)
                    : 0
            }
        };
    }

    private List<WeeklyTrend> BuildTrends(List<StudentProcessedData> studentsData)
    {
        var weeklyData = new Dictionary<string, WeeklyActivityData>();

        foreach (var student in studentsData)
        {
            foreach (var activity in student.AllActivities)
            {
                var week = GetWeekIdentifier(activity.Date);
                
                if (!weeklyData.ContainsKey(week))
                    weeklyData[week] = new WeeklyActivityData();

                weeklyData[week].ActiveStudents.Add(student.Student.Id);
                weeklyData[week].SubmissionCount++;
                
                if (activity.Grade.HasValue)
                    weeklyData[week].Grades.Add(activity.Grade.Value);
            }
        }

        var totalStudents = studentsData.Count;
        return weeklyData
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => new WeeklyTrend
            {
                Week = kvp.Key,
                ActiveStudents = kvp.Value.ActiveStudents.Count,
                SubmissionCount = kvp.Value.SubmissionCount,
                CompletionRate = totalStudents > 0
                    ? Math.Round((decimal)kvp.Value.ActiveStudents.Count / totalStudents * 100, 2)
                    : 0,
                AverageGrade = kvp.Value.Grades.Any() 
                    ? Math.Round(kvp.Value.Grades.Average(), 2) 
                    : 0
            })
            .ToList();
    }

    private List<StudentPerformanceItem> BuildStudentPerformanceList(
        List<StudentProcessedData> studentsData,
        int totalAssignments)
    {
        return studentsData.Select(s =>
        {
            var allGrades = s.TrainingGrades.Concat(s.GradingGrades).Concat(s.QuizScores).ToList();
            var completionRate = totalAssignments > 0
                ? Math.Round((decimal)s.TotalSolutions / totalAssignments * 100, 2)
                : 0;

            return new StudentPerformanceItem
            {
                StudentId = s.Student.Id.ToString(),
                StudentName = s.Student.Name,
                Email = s.Student.Email,
                OverallGrade = allGrades.Any() ? Math.Round(allGrades.Average(), 2) : 0,
                CompletedAssignments = s.TotalSolutions,
                TotalAssignments = totalAssignments,
                CompletionRate = completionRate,
                QuizAverage = s.QuizScores.Any() ? Math.Round(s.QuizScores.Average(), 2) : 0,
                QuizzesCompleted = s.QuizCount,
                LastActivity = s.LastActivity
            };
        })
        .OrderByDescending(s => s.OverallGrade)
        .ToList();
    }

    private List<AtRiskStudent> BuildAtRiskStudents(
        List<StudentProcessedData> studentsData,
        int totalAssignments)
    {
        var atRisk = new List<AtRiskStudent>();

        foreach (var s in studentsData)
        {
            var riskFactors = new List<string>();
            var allGrades = s.TrainingGrades.Concat(s.GradingGrades).Concat(s.QuizScores).ToList();
            var avgGrade = allGrades.Any() ? allGrades.Average() : 0;
            var completionRate = totalAssignments > 0
                ? Math.Round((decimal)s.TotalSolutions / totalAssignments * 100, 2)
                : 0;

            if (completionRate < 50)
                riskFactors.Add("Low completion rate");

            if (avgGrade < 60 && avgGrade > 0)
                riskFactors.Add("Below average performance");

            if (s.TotalSolutions == 0 && s.QuizCount == 0)
                riskFactors.Add("No submissions");

            var daysSinceLastActivity = (DateTimeOffset.UtcNow - s.LastActivity).Days;
            if (daysSinceLastActivity > 14 && s.LastActivity != DateTimeOffset.MinValue)
                riskFactors.Add("Inactive for 2+ weeks");

            if (riskFactors.Any())
            {
                atRisk.Add(new AtRiskStudent
                {
                    StudentId = s.Student.Id.ToString(),
                    StudentName = s.Student.Name,
                    Email = s.Student.Email,
                    CompletionRate = completionRate,
                    OverallGrade = avgGrade,
                    RiskFactors = riskFactors
                });
            }
        }

        return atRisk.OrderBy(s => s.CompletionRate).ToList();
    }

    // ===== HELPERS =====

    private CourseFullAnalyticsDto CreateEmptyAnalytics(CourseInfoSection courseInfo)
    {
        return new CourseFullAnalyticsDto
        {
            CourseInfo = courseInfo,
            Kpis = new KPIsSection(),
            Engagement = new EngagementSection { ActivityByType = new Dictionary<string, int>() },
            Performance = new PerformanceSection
            {
                TrainingMode = new TrainingModeStats(),
                GradingMode = new GradingModeStats { ScoresByType = new Dictionary<string, decimal>() },
                Comparison = new ComparisonStats()
            },
            Trends = new List<WeeklyTrend>(),
            StudentPerformance = new List<StudentPerformanceItem>(),
            AtRiskStudents = new List<AtRiskStudent>(),
            ResourceUsage = new ResourceUsageSection { MostAccessedResources = new List<ResourceAccessItem>() },
            GeneratedAt = DateTimeOffset.UtcNow
        };
    }

    private List<decimal> ParseGrades(List<string> grades)
    {
        return grades
            .Where(g => !string.IsNullOrEmpty(g) && decimal.TryParse(g, out _))
            .Select(g => decimal.Parse(g))
            .Where(g => g > 0)
            .ToList();
    }

    private decimal? ParseSingleGrade(string grade)
    {
        if (string.IsNullOrEmpty(grade)) return null;
        return decimal.TryParse(grade, out var parsed) ? parsed : null;
    }

    private string GetWeekIdentifier(DateTimeOffset date)
    {
        var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
        var weekNum = calendar.GetWeekOfYear(
            date.DateTime, 
            System.Globalization.CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);
        return $"{date.Year}-W{weekNum:D2}";
    }
}

// ===== INTERNAL DATA CLASSES =====

internal class StudentBasicInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

internal class SolutionData
{
    public Guid UserId { get; set; }
    public string Grade { get; set; }
    public bool IsTraining { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public string AssignmentType { get; set; }
}

internal class ExamResultData
{
    public Guid StudentId { get; set; }
    public decimal Score { get; set; }
    public DateTimeOffset FinishedAt { get; set; }
    public double Duration { get; set; }
}

internal class MongoDataSummary
{
    public int TotalSessions { get; set; }
    public int CompletedSessions { get; set; }
    public int TotalAnswers { get; set; }
}

internal class StudentProcessedData
{
    public StudentBasicInfo Student { get; set; }
    public int TotalSolutions { get; set; }
    public int TrainingSolutions { get; set; }
    public int GradingSolutions { get; set; }
    public int QuizCount { get; set; }
    public List<decimal> TrainingGrades { get; set; }
    public List<decimal> GradingGrades { get; set; }
    public List<decimal> QuizScores { get; set; }
    public DateTimeOffset LastActivity { get; set; }
    public List<ActivityData> AllActivities { get; set; }
}

internal class ActivityData
{
    public DateTimeOffset Date { get; set; }
    public decimal? Grade { get; set; }
}

internal class WeeklyActivityData
{
    public HashSet<Guid> ActiveStudents { get; set; } = new();
    public int SubmissionCount { get; set; }
    public List<decimal> Grades { get; set; } = new();
}

// DTOs
public class CourseFullAnalyticsDto
{
    public CourseInfoSection CourseInfo { get; set; }
    public KPIsSection Kpis { get; set; }
    public EngagementSection Engagement { get; set; }
    public PerformanceSection Performance { get; set; }
    public List<WeeklyTrend> Trends { get; set; }
    public List<StudentPerformanceItem> StudentPerformance { get; set; }
    public List<AtRiskStudent> AtRiskStudents { get; set; }
    public ResourceUsageSection ResourceUsage { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
}

public class CourseInfoSection
{
    public int TotalStudents { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalQuizzes { get; set; }
    public int TotalTopics { get; set; }
    public int TotalModules { get; set; }
}

public class KPIsSection
{
    public decimal AverageGrade { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal PassRate { get; set; }
    public int TotalSubmissions { get; set; }
    public decimal EngagementScore { get; set; }
}

public class EngagementSection
{
    public decimal AverageAttemptsPerStudent { get; set; }
    public decimal DropOffRate { get; set; }
    public double AverageTimePerAssessment { get; set; }
    public int TotalInteractions { get; set; }
    public decimal ActiveStudentsPercentage { get; set; }
    public Dictionary<string, int> ActivityByType { get; set; }
}

public class PerformanceSection
{
    public TrainingModeStats TrainingMode { get; set; }
    public GradingModeStats GradingMode { get; set; }
    public ComparisonStats Comparison { get; set; }
}

public class TrainingModeStats
{
    public decimal AverageScore { get; set; }
    public decimal AverageImprovement { get; set; }
    public int TotalAttempts { get; set; }
    public decimal PassRate { get; set; }
}

public class GradingModeStats
{
    public decimal AverageScore { get; set; }
    public decimal PassRate { get; set; }
    public int TotalSubmissions { get; set; }
    public Dictionary<string, decimal> ScoresByType { get; set; }
}

public class ComparisonStats
{
    public decimal TrainingAverage { get; set; }
    public int TrainingCount { get; set; }
    public decimal GradingAverage { get; set; }
    public int GradingCount { get; set; }
    public decimal ImprovementRate { get; set; }
}

public class WeeklyTrend
{
    public string Week { get; set; }
    public int ActiveStudents { get; set; }
    public int SubmissionCount { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageGrade { get; set; }
}

public class StudentPerformanceItem
{
    public string StudentId { get; set; }
    public string StudentName { get; set; }
    public string Email { get; set; }
    public decimal OverallGrade { get; set; }
    public int CompletedAssignments { get; set; }
    public int TotalAssignments { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal QuizAverage { get; set; }
    public int QuizzesCompleted { get; set; }
    public DateTimeOffset LastActivity { get; set; }
}

public class AtRiskStudent
{
    public string StudentId { get; set; }
    public string StudentName { get; set; }
    public string Email { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal OverallGrade { get; set; }
    public List<string> RiskFactors { get; set; }
}

public class ResourceUsageSection
{
    public int TotalResourcesAccessed { get; set; }
    public double AverageTimeSpent { get; set; }
    public List<ResourceAccessItem> MostAccessedResources { get; set; }
}

public class ResourceAccessItem
{
    public string ResourceId { get; set; }
    public string ResourceTitle { get; set; }
    public int AccessCount { get; set; }
}