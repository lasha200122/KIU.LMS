using Microsoft.Extensions.Logging;

namespace KIU.LMS.Application.Features.Analytics.Queries;

public sealed record StudentCourseAnalyticsQuery(Guid StudentId, Guid CourseId) 
    : IRequest<Result<StudentCourseAnalyticsDto>>;

public sealed class StudentCourseAnalyticsQueryHandler(
    IUnitOfWork _unitOfWork,
    ILogger<StudentCourseAnalyticsQueryHandler> _logger) 
    : IRequestHandler<StudentCourseAnalyticsQuery, Result<StudentCourseAnalyticsDto>>
{
    public async Task<Result<StudentCourseAnalyticsDto>> Handle(
        StudentCourseAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await GetStudentCourseAnalyticsAsync(request.StudentId, request.CourseId, cancellationToken);
            return dto != null
                ? Result<StudentCourseAnalyticsDto>.Success(dto)
                : Result<StudentCourseAnalyticsDto>.Failure("Student or course not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course analytics for student {StudentId} in course {CourseId}", 
                request.StudentId, request.CourseId);
            return Result<StudentCourseAnalyticsDto>.Failure(
                $"An error occurred while fetching course analytics: {ex.Message}");
        }
    }

    private async Task<StudentCourseAnalyticsDto> GetStudentCourseAnalyticsAsync(
        Guid studentId, Guid courseId, CancellationToken ct)
    {
        var user = await _unitOfWork.UserRepository
            .FirstOrDefaultMappedAsync(
                u => u.Id == studentId && u.Role == UserRole.Student,
                u => new { u.Id, u.Role },
                ct);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found or not a student", studentId);
            return null;
        }

        var courseExists = await _unitOfWork.CourseRepository.ExistsAsync(c => c.Id == courseId, ct);
        if (!courseExists)
        {
            _logger.LogWarning("Course {CourseId} not found", courseId);
            return null;
        }

        var userCourse = await _unitOfWork.UserCourseRepository
            .FirstOrDefaultMappedAsync(
                uc => uc.UserId == studentId && uc.CourseId == courseId,
                uc => new { uc.CreateDate },
                ct);
        
        if (userCourse == null)
        {
            _logger.LogWarning("Student {StudentId} not enrolled in course {CourseId}", 
                studentId, courseId);
            return null;
        }

        _logger.LogInformation("Loading analytics data for student {StudentId} in course {CourseId}", 
            studentId, courseId);

        var courseInfo = await BuildCourseInfo(courseId, userCourse.CreateDate, ct);
        var assignmentStats = await BuildAssignmentStatistics(studentId, courseId, ct);
        var quizStats = await BuildQuizStatistics(studentId, courseId, ct);
        var topicProgress = await BuildTopicProgress(studentId, courseId, ct);
        var moduleProgress = await BuildModuleProgress(courseId, ct);
        var performance = await BuildCoursePerformance(studentId, courseId, ct);

        return new StudentCourseAnalyticsDto
        {
            StudentId = studentId,
            CourseId = courseId,
            CourseInfo = courseInfo,
            AssignmentStatistics = assignmentStats,
            QuizStatistics = quizStats,
            TopicProgress = topicProgress,
            ModuleProgress = moduleProgress,
            Performance = performance,
            GeneratedAt = DateTimeOffset.UtcNow
        };
    }
    
    private async Task<CourseInfoDto> BuildCourseInfo(
        Guid courseId, 
        DateTimeOffset enrolledAt,
        CancellationToken ct)
    {
        try
        {
            var courseName = await _unitOfWork.CourseRepository
                .FirstOrDefaultMappedAsync(
                    c => c.Id == courseId,
                    c => c.Name,
                    ct);

            var materialsCount = await _unitOfWork.CourseMaterialRepository
                .CountWithPredicateAsync(cm => cm.CourseId == courseId, ct);
            
            var meetingsCount = await _unitOfWork.CourseMeetingRepository
                .CountWithPredicateAsync(cm => cm.CourseId == courseId, ct);
            
            var assignmentsCount = await _unitOfWork.AssignmentRepository
                .CountWithPredicateAsync(a => a.CourseId == courseId, ct);
            
            var quizzesCount = await _unitOfWork.QuizRepository
                .CountWithPredicateAsync(q => q.CourseId == courseId, ct);
            
            var topicsCount = await _unitOfWork.TopicRepository
                .CountWithPredicateAsync(t => t.CourseId == courseId, ct);
            
            var modulesCount = await _unitOfWork.ModuleRepository
                .CountWithPredicateAsync(m => m.CourseId == courseId, ct);

            return new CourseInfoDto
            {
                CourseName = courseName ?? "Unknown Course",
                EnrolledAt = enrolledAt,
                TotalMaterials = materialsCount,
                TotalMeetings = meetingsCount,
                TotalAssignments = assignmentsCount,
                TotalQuizzes = quizzesCount,
                TotalTopics = topicsCount,
                TotalModules = modulesCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building course info for course {CourseId}", courseId);
            return new CourseInfoDto { CourseName = "Unknown" };
        }
    }

    private async Task<AssignmentStatisticsDto> BuildAssignmentStatistics(
        Guid studentId, 
        Guid courseId,
        CancellationToken ct)
    {
        try
        {
            var assignments = await _unitOfWork.AssignmentRepository
                .GetMappedAsync(
                    a => a.CourseId == courseId,
                    a => new AssignmentMinimalDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Type = a.Type.ToString(),
                        EndDateTime = a.EndDateTime,
                        IsTraining = a.IsTraining,
                        TopicId = a.TopicId
                    },
                    ct);

            var assignmentsList = assignments.ToList();
            
            if (!assignmentsList.Any())
            {
                return new AssignmentStatisticsDto 
                { 
                    Submissions = new List<AssignmentSubmissionDetailDto>(),
                    SubmissionsByType = new Dictionary<string, int>(),
                    AverageGradeByType = new Dictionary<string, decimal>()
                };
            }

            var assignmentIds = assignmentsList.Select(a => a.Id).ToList();

            var solutions = await _unitOfWork.SolutionRepository
                .GetMappedAsync(
                    s => assignmentIds.Contains(s.AssignmentId) && s.UserId == studentId,
                    s => new SolutionMinimalDto
                    {
                        AssignmentId = s.AssignmentId,
                        Grade = s.Grade,
                        GradingStatus = s.GradingStatus.ToString(),
                        FeedBack = s.FeedBack,
                        CreateDate = s.CreateDate
                    },
                    ct);

            var solutionsList = solutions.ToList();
            var solutionsByAssignmentId = solutionsList.ToDictionary(s => s.AssignmentId);

            var submissionDetails = new List<AssignmentSubmissionDetailDto>();
            var submissionsByType = new Dictionary<string, int>();
            var gradesByType = new Dictionary<string, List<decimal>>();

            foreach (var assignment in assignmentsList)
            {
                if (solutionsByAssignmentId.TryGetValue(assignment.Id, out var solution))
                {
                    var typeName = assignment.Type;
                    
                    submissionsByType[typeName] = submissionsByType.GetValueOrDefault(typeName, 0) + 1;

                    if (!string.IsNullOrEmpty(solution.Grade) && 
                        decimal.TryParse(solution.Grade, out var gradeValue))
                    {
                        if (!gradesByType.ContainsKey(typeName))
                            gradesByType[typeName] = new List<decimal>();
                        gradesByType[typeName].Add(gradeValue);
                    }

                    var isLate = assignment.EndDateTime.HasValue && 
                                solution.CreateDate > assignment.EndDateTime.Value;

                    submissionDetails.Add(new AssignmentSubmissionDetailDto
                    {
                        AssignmentId = assignment.Id,
                        AssignmentName = assignment.Name ?? "Unknown Assignment",
                        Type = typeName,
                        SubmittedAt = solution.CreateDate,
                        Grade = solution.Grade,
                        GradingStatus = solution.GradingStatus,
                        Feedback = solution.FeedBack,
                        IsLate = isLate,
                        IsTraining = assignment.IsTraining,
                        DueDate = assignment.EndDateTime
                    });
                }
            }

            var gradedCount = solutionsList.Count(s => s.GradingStatus == "Completed");
            var pendingCount = solutionsList.Count(s => s.GradingStatus == "InProgress");
            var allGrades = gradesByType.SelectMany(kvp => kvp.Value).ToList();
            var averageGrade = allGrades.Any() ? Math.Round(allGrades.Average(), 2) : 0;

            var averageByType = gradesByType.ToDictionary(
                kvp => kvp.Key,
                kvp => Math.Round(kvp.Value.Average(), 2)
            );

            return new AssignmentStatisticsDto
            {
                TotalAssignments = assignmentsList.Count,
                SubmittedAssignments = solutionsList.Count,
                GradedAssignments = gradedCount,
                PendingGrading = pendingCount,
                AverageGrade = averageGrade,
                SubmissionsByType = submissionsByType,
                AverageGradeByType = averageByType,
                OnTimeSubmissions = submissionDetails.Count(s => !s.IsLate),
                LateSubmissions = submissionDetails.Count(s => s.IsLate),
                Submissions = submissionDetails.OrderByDescending(s => s.SubmittedAt).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building assignment statistics for student {StudentId}, course {CourseId}", 
                studentId, courseId);
            return new AssignmentStatisticsDto 
            { 
                Submissions = new List<AssignmentSubmissionDetailDto>(),
                SubmissionsByType = new Dictionary<string, int>(),
                AverageGradeByType = new Dictionary<string, decimal>()
            };
        }
    }

    private async Task<QuizStatisticsDto> BuildQuizStatistics(
        Guid studentId, 
        Guid courseId,
        CancellationToken ct)
    {
        try
        {
            var quizzes = await _unitOfWork.QuizRepository
                .GetMappedAsync(
                    q => q.CourseId == courseId,
                    q => new { q.Id, q.Title },
                    ct);

            var quizzesList = quizzes.ToList();
            var quizIds = quizzesList.Select(q => q.Id).ToList();

            if (!quizIds.Any())
            {
                return new QuizStatisticsDto { QuizResults = new List<QuizResultDetailDto>() };
            }

            var examResults = await _unitOfWork.ExamResultRepository
                .GetMappedAsync(
                    er => quizIds.Contains(er.QuizId) && er.StudentId == studentId,
                    er => new ExamResultMinimalDto
                    {
                        QuizId = er.QuizId,
                        Score = er.Score,
                        TotalQuestions = er.TotalQuestions,
                        CorrectAnswers = er.CorrectAnswers,
                        StartedAt = er.StartedAt,
                        FinishedAt = er.FinishedAt,
                        Duration = er.Duration
                    },
                    ct);

            var examResultsList = examResults.ToList();

            if (!examResultsList.Any())
            {
                return new QuizStatisticsDto 
                { 
                    TotalQuizzes = quizzesList.Count,
                    QuizResults = new List<QuizResultDetailDto>() 
                };
            }

            var quizzesById = quizzesList.ToDictionary(q => q.Id, q => q.Title);
            
            var quizDetails = examResultsList
                .Select(result => new QuizResultDetailDto
                {
                    QuizId = result.QuizId,
                    QuizName = quizzesById.GetValueOrDefault(result.QuizId, "Unknown Quiz") ?? "Unknown Quiz",
                    TakenAt = result.StartedAt,
                    FinishedAt = result.FinishedAt,
                    Score = result.Score,
                    TotalQuestions = result.TotalQuestions,
                    CorrectAnswers = result.CorrectAnswers,
                    Duration = result.Duration,
                    PerformanceLevel = DeterminePerformanceLevel(result.Score)
                })
                .OrderByDescending(q => q.TakenAt)
                .ToList();

            var averageScore = Math.Round(examResultsList.Average(e => e.Score), 2);
            var highestScore = examResultsList.Max(e => e.Score);
            var lowestScore = examResultsList.Min(e => e.Score);

            return new QuizStatisticsDto
            {
                TotalQuizzes = quizzesList.Count,
                CompletedQuizzes = examResultsList.Count,
                AverageScore = averageScore,
                HighestScore = highestScore,
                LowestScore = lowestScore,
                QuizResults = quizDetails
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building quiz statistics for student {StudentId}, course {CourseId}", 
                studentId, courseId);
            return new QuizStatisticsDto { QuizResults = new List<QuizResultDetailDto>() };
        }
    }

    private async Task<List<TopicProgressDetailDto>> BuildTopicProgress(
        Guid studentId, 
        Guid courseId,
        CancellationToken ct)
    {
        try
        {
            var topics = await _unitOfWork.TopicRepository
                .GetMappedAsync(
                    t => t.CourseId == courseId,
                    t => new { t.Id, t.Name },
                    ct);

            var topicsList = topics.ToList();
            if (!topicsList.Any())
                return new List<TopicProgressDetailDto>();

            var topicIds = topicsList.Select(t => t.Id).ToList();

            var assignments = await _unitOfWork.AssignmentRepository
                .GetMappedAsync(
                    a => topicIds.Contains(a.TopicId),
                    a => new { a.Id, a.TopicId },
                    ct);

            var assignmentsList = assignments.ToList();
            var assignmentIds = assignmentsList.Select(a => a.Id).ToList();

            var solutions = await _unitOfWork.SolutionRepository
                .GetMappedAsync(
                    s => assignmentIds.Contains(s.AssignmentId) && s.UserId == studentId,
                    s => new { s.AssignmentId, s.Grade },
                    ct);

            var solutionsList = solutions.ToList();
            var assignmentsByTopic = assignmentsList
                .GroupBy(a => a.TopicId)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Id).ToList());

            var solutionsByAssignmentId = solutionsList.ToLookup(s => s.AssignmentId);

            return topicsList.Select(topic =>
            {
                var topicAssignmentIds = assignmentsByTopic.GetValueOrDefault(topic.Id, new List<Guid>());
                
                var completedAssignments = topicAssignmentIds
                    .Count(aId => solutionsByAssignmentId[aId].Any());

                var topicGrades = topicAssignmentIds
                    .SelectMany(aId => solutionsByAssignmentId[aId])
                    .Where(s => !string.IsNullOrEmpty(s.Grade))
                    .Select(s => decimal.TryParse(s.Grade, out var g) ? g : 0)
                    .Where(g => g > 0)
                    .ToList();

                return new TopicProgressDetailDto
                {
                    TopicId = topic.Id,
                    TopicName = topic.Name ?? "Unknown Topic",
                    TotalAssignments = topicAssignmentIds.Count,
                    CompletedAssignments = completedAssignments,
                    AverageGrade = topicGrades.Any() ? Math.Round(topicGrades.Average(), 2) : 0,
                    CompletionRate = topicAssignmentIds.Any() 
                        ? Math.Round((decimal)completedAssignments / topicAssignmentIds.Count * 100, 2) 
                        : 0
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building topic progress for student {StudentId}, course {CourseId}", 
                studentId, courseId);
            return new List<TopicProgressDetailDto>();
        }
    }

    private async Task<List<ModuleProgressDetailDto>> BuildModuleProgress(
        Guid courseId,
        CancellationToken ct)
    {
        try
        {
            var modules = await _unitOfWork.ModuleRepository
                .GetMappedAsync(
                    m => m.CourseId == courseId,
                    m => new { m.Id, m.Name },
                    ct);
            
            return modules.Select(module => new ModuleProgressDetailDto
            {
                ModuleId = module.Id,
                ModuleName = module.Name ?? "Unknown Module",
                IsCompleted = false // TODO: Implement completion logic
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building module progress for course {CourseId}", courseId);
            return new List<ModuleProgressDetailDto>();
        }
    }

    private async Task<CoursePerformanceDto> BuildCoursePerformance(
        Guid studentId, 
        Guid courseId,
        CancellationToken ct)
    {
        try
        {
            var solutions = await _unitOfWork.SolutionRepository
                .GetMappedAsync(
                    s => s.UserId == studentId && s.Assignment.CourseId == courseId,
                    s => new { s.Grade, s.CreateDate },
                    ct);

            var examResults = await _unitOfWork.ExamResultRepository
                .GetMappedAsync(
                    er => er.StudentId == studentId && er.Quiz.CourseId == courseId,
                    er => new { er.Score, er.FinishedAt },
                    ct);

            var solutionsList = solutions.ToList();
            var examResultsList = examResults.ToList();

            var solutionGrades = solutionsList
                .Where(s => !string.IsNullOrEmpty(s.Grade) && decimal.TryParse(s.Grade, out _))
                .Select(s => new { Grade = decimal.Parse(s.Grade), Date = s.CreateDate })
                .ToList();

            var examGrades = examResultsList
                .Select(e => new { Grade = e.Score, Date = e.FinishedAt })
                .ToList();

            var allGrades = solutionGrades.Concat(examGrades).ToList();
            var overallGrade = allGrades.Any() 
                ? Math.Round(allGrades.Average(g => g.Grade), 2) 
                : 0;

            var trend = AnalyzeTrend([allGrades]);
            var lastActivity = allGrades.Any() 
                ? allGrades.Max(g => g.Date) 
                : DateTimeOffset.MinValue;

            return new CoursePerformanceDto
            {
                OverallGrade = overallGrade,
                PerformanceTrend = trend,
                TotalActivities = solutionsList.Count + examResultsList.Count,
                LastActivityDate = lastActivity
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building course performance for student {StudentId}, course {CourseId}", 
                studentId, courseId);
            return new CoursePerformanceDto();
        }
    }

    // ===== HELPER METHODS =====

    private string DeterminePerformanceLevel(decimal score)
    {
        return score switch
        {
            >= 90 => "Excellent",
            >= 75 => "Good",
            >= 60 => "Average",
            _ => "Needs Improvement"
        };
    }

    private string AnalyzeTrend(List<dynamic> allGrades)
    {
        if (!allGrades.Any())
            return "Stable";

        var recentDate = DateTimeOffset.UtcNow.AddDays(-30);
        
        var recentGrades = allGrades.Where(g => g.Date >= recentDate).Select(g => (decimal)g.Grade).ToList();
        var olderGrades = allGrades.Where(g => g.Date < recentDate).Select(g => (decimal)g.Grade).ToList();

        if (!recentGrades.Any() || !olderGrades.Any())
            return "Stable";

        var recentAvg = recentGrades.Average();
        var olderAvg = olderGrades.Average();
        var difference = recentAvg - olderAvg;

        return difference switch
        {
            > 5 => "Improving",
            < -5 => "Declining",
            _ => "Stable"
        };
    }
}

internal class AssignmentMinimalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTimeOffset? EndDateTime { get; set; }
    public bool IsTraining { get; set; }
    public Guid TopicId { get; set; }
}

internal class SolutionMinimalDto
{
    public Guid AssignmentId { get; set; }
    public string Grade { get; set; }
    public string GradingStatus { get; set; }
    public string FeedBack { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}

internal class ExamResultMinimalDto
{
    public Guid QuizId { get; set; }
    public decimal Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset FinishedAt { get; set; }
    public TimeSpan Duration { get; set; }
}

// ===== PUBLIC DTOs =====

public class StudentCourseAnalyticsDto
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public CourseInfoDto CourseInfo { get; set; }
    public AssignmentStatisticsDto AssignmentStatistics { get; set; }
    public QuizStatisticsDto QuizStatistics { get; set; }
    public List<TopicProgressDetailDto> TopicProgress { get; set; }
    public List<ModuleProgressDetailDto> ModuleProgress { get; set; }
    public CoursePerformanceDto Performance { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
}

public class CourseInfoDto
{
    public string CourseName { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
    public int TotalMaterials { get; set; }
    public int TotalMeetings { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalQuizzes { get; set; }
    public int TotalTopics { get; set; }
    public int TotalModules { get; set; }
}

public class AssignmentStatisticsDto
{
    public int TotalAssignments { get; set; }
    public int SubmittedAssignments { get; set; }
    public int GradedAssignments { get; set; }
    public int PendingGrading { get; set; }
    public decimal AverageGrade { get; set; }
    public Dictionary<string, int> SubmissionsByType { get; set; } = new();
    public Dictionary<string, decimal> AverageGradeByType { get; set; } = new();
    public int OnTimeSubmissions { get; set; }
    public int LateSubmissions { get; set; }
    public List<AssignmentSubmissionDetailDto> Submissions { get; set; } = new();
}

public class AssignmentSubmissionDetailDto
{
    public Guid AssignmentId { get; set; }
    public string AssignmentName { get; set; }
    public string Type { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
    public string Grade { get; set; }
    public string GradingStatus { get; set; }
    public string Feedback { get; set; }
    public bool IsLate { get; set; }
    public bool IsTraining { get; set; }
    public DateTimeOffset? DueDate { get; set; }
}

public class QuizStatisticsDto
{
    public int TotalQuizzes { get; set; }
    public int CompletedQuizzes { get; set; }
    public decimal AverageScore { get; set; }
    public decimal HighestScore { get; set; }
    public decimal LowestScore { get; set; }
    public List<QuizResultDetailDto> QuizResults { get; set; } = new();
}

public class QuizResultDetailDto
{
    public Guid QuizId { get; set; }
    public string QuizName { get; set; }
    public DateTimeOffset TakenAt { get; set; }
    public DateTimeOffset FinishedAt { get; set; }
    public decimal Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public TimeSpan Duration { get; set; }
    public string PerformanceLevel { get; set; }
}

public class TopicProgressDetailDto
{
    public Guid TopicId { get; set; }
    public string TopicName { get; set; }
    public int TotalAssignments { get; set; }
    public int CompletedAssignments { get; set; }
    public decimal AverageGrade { get; set; }
    public decimal CompletionRate { get; set; }
}

public class ModuleProgressDetailDto
{
    public Guid ModuleId { get; set; }
    public string ModuleName { get; set; }
    public bool IsCompleted { get; set; }
}

public class CoursePerformanceDto
{
    public decimal OverallGrade { get; set; }
    public string PerformanceTrend { get; set; }
    public int TotalActivities { get; set; }
    public DateTimeOffset LastActivityDate { get; set; }
}
