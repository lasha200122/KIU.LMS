using KIU.LMS.Domain.Entities.NoSQL;
using Microsoft.Extensions.Logging;

namespace KIU.LMS.Application.Features.Analytics.Queries;

public sealed record StudentCourseAnalyticsQuery(Guid StudentId, Guid CourseId) 
    : IRequest<Result<StudentCourseAnalyticsDto>>;

public sealed class StudentCourseAnalyticsQueryHandler(
    IUnitOfWork _unitOfWork,
    IMongoRepository<ExamSession> _examSessionRepository,
    IMongoRepository<StudentAnswer> _studentAnswerRepository,
    IMongoRepository<Question> _questionRepository,
    ILogger<StudentCourseAnalyticsQueryHandler> _logger) 
    : IRequestHandler<StudentCourseAnalyticsQuery, Result<StudentCourseAnalyticsDto>>
{
    public async Task<Result<StudentCourseAnalyticsDto>> Handle(
        StudentCourseAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = await GetStudentCourseAnalyticsAsync(request.StudentId, request.CourseId);
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
        Guid studentId, Guid courseId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(studentId);
        if (user == null || user.Role != UserRole.Student)
        {
            _logger.LogWarning("User {UserId} not found or not a student", studentId);
            return null;
        }

        var course = await _unitOfWork.CourseRepository.GetByIdWithDetailsAsync(courseId);
        if (course == null)
        {
            _logger.LogWarning("Course {CourseId} not found", courseId);
            return null;
        }

        var userCourse = await _unitOfWork.UserCourseRepository
            .FirstOrDefaultAsync(x => x.UserId == studentId && x.CourseId == courseId);
        
        if (userCourse == null)
        {
            _logger.LogWarning("Student {StudentId} not enrolled in course {CourseId}", 
                studentId, courseId);
            return null;
        }

        var courseInfoTask = BuildCourseInfoAsync(course, userCourse);
        var assignmentStatsTask = BuildAssignmentStatisticsAsync(studentId, courseId);
        var quizStatsTask = BuildQuizStatisticsAsync(studentId, courseId);
        var topicProgressTask = BuildTopicProgressAsync(studentId, courseId);
        var moduleProgressTask = BuildModuleProgressAsync(studentId, courseId);
        var performanceTask = BuildCoursePerformanceAsync(studentId, courseId);

        await Task.WhenAll(
            courseInfoTask,
            assignmentStatsTask,
            quizStatsTask,
            topicProgressTask,
            moduleProgressTask,
            performanceTask
        );

        return new StudentCourseAnalyticsDto
        {
            StudentId = studentId,
            CourseId = courseId,
            CourseInfo = await courseInfoTask,
            AssignmentStatistics = await assignmentStatsTask,
            QuizStatistics = await quizStatsTask,
            TopicProgress = await topicProgressTask,
            ModuleProgress = await moduleProgressTask,
            Performance = await performanceTask,
            GeneratedAt = DateTimeOffset.UtcNow
        };
    }

    private async Task<CourseInfoDto> BuildCourseInfoAsync(Course course, UserCourse userCourse)
    {
        try
        {
            return new CourseInfoDto
            {
                CourseName = course.Name,
                EnrolledAt = userCourse.CreateDate,
                TotalMaterials = course.Materials.Count,
                TotalMeetings = course.Meetings.Count,
                TotalAssignments = course.Assignments.Count,
                TotalQuizzes = course.Quizzes.Count,
                TotalTopics = course.Topics.Count,
                TotalModules = course.Modules.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building course info");
            return new CourseInfoDto { CourseName = course.Name };
        }
    }

    private async Task<AssignmentStatisticsDto> BuildAssignmentStatisticsAsync(
        Guid studentId, Guid courseId)
    {
        try
        {
            var assignments = await _unitOfWork.AssignmentRepository
                .GetByCourseIdAsync(courseId) ?? Enumerable.Empty<Assignment>();
            var solutions = await _unitOfWork.SolutionRepository
                .GetByUserAndCourseAsync(studentId, courseId) ?? Enumerable.Empty<Solution>();

            var assignmentsList = assignments.ToList();
            var solutionsList = solutions.ToList();

            var submissionDetails = new List<AssignmentSubmissionDetailDto>();
            var submissionsByType = new Dictionary<string, int>();
            var gradesByType = new Dictionary<string, List<decimal>>();

            foreach (var assignment in assignmentsList)
            {
                var solution = solutionsList.FirstOrDefault(s => s.AssignmentId == assignment.Id);
                var typeName = assignment.Type.ToString();

                if (solution != null)
                {
                    if (!submissionsByType.ContainsKey(typeName))
                        submissionsByType[typeName] = 0;
                    submissionsByType[typeName]++;

                    if (!string.IsNullOrEmpty(solution.Grade) && 
                        decimal.TryParse(solution.Grade, out var gradeValue))
                    {
                        if (!gradesByType.ContainsKey(typeName))
                            gradesByType[typeName] = new List<decimal>();
                        gradesByType[typeName].Add(gradeValue);
                    }

                    submissionDetails.Add(new AssignmentSubmissionDetailDto
                    {
                        AssignmentId = assignment.Id,
                        AssignmentName = assignment.Name,
                        Type = typeName,
                        SubmittedAt = solution.CreateDate,
                        Grade = solution.Grade,
                        GradingStatus = solution.GradingStatus.ToString(),
                        Feedback = solution.FeedBack,
                        IsLate = assignment.EndDateTime.HasValue && 
                                solution.CreateDate > assignment.EndDateTime.Value,
                        IsTraining = assignment.IsTraining,
                        DueDate = assignment.EndDateTime
                    });
                }
            }

            var gradedCount = solutionsList.Count(s => s.GradingStatus == GradingStatus.Completed);
            var pendingCount = solutionsList.Count(s => s.GradingStatus == GradingStatus.InProgress);
            var allGrades = gradesByType.SelectMany(kvp => kvp.Value).ToList();
            var averageGrade = allGrades.Any() ? Math.Round(allGrades.Average(), 2) : 0;

            var averageByType = gradesByType.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Any() ? Math.Round(kvp.Value.Average(), 2) : 0
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
            _logger.LogError(ex, "Error building assignment statistics");
            return new AssignmentStatisticsDto();
        }
    }

    private async Task<QuizStatisticsDto> BuildQuizStatisticsAsync(
        Guid studentId, Guid courseId)
    {
        try
        {
            var quizzes = await _unitOfWork.QuizRepository
                .GetByCourseIdAsync(courseId) ?? Enumerable.Empty<Quiz>();
            var examResults = await _unitOfWork.ExamResultRepository
                .GetByUserAndCourseAsync(studentId, courseId) ?? Enumerable.Empty<ExamResult>();

            var quizzesList = quizzes.ToList();
            var examResultsList = examResults.ToList();

            var quizDetails = new List<QuizResultDetailDto>();

            foreach (var result in examResultsList)
            {
                var quiz = quizzesList.FirstOrDefault(q => q.Id == result.QuizId);
                if (quiz != null)
                {
                    quizDetails.Add(new QuizResultDetailDto
                    {
                        QuizId = result.QuizId,
                        QuizName = quiz.Title,
                        TakenAt = result.StartedAt,
                        FinishedAt = result.FinishedAt,
                        Score = result.Score,
                        TotalQuestions = result.TotalQuestions,
                        CorrectAnswers = result.CorrectAnswers,
                        Duration = result.Duration,
                        PerformanceLevel = DeterminePerformanceLevel(result.Score)
                    });
                }
            }

            var averageScore = examResultsList.Any() 
                ? Math.Round(examResultsList.Average(e => e.Score), 2) : 0;
            var highestScore = examResultsList.Any() ? examResultsList.Max(e => e.Score) : 0;
            var lowestScore = examResultsList.Any() ? examResultsList.Min(e => e.Score) : 0;

            return new QuizStatisticsDto
            {
                TotalQuizzes = quizzesList.Count,
                CompletedQuizzes = examResultsList.Count,
                AverageScore = averageScore,
                HighestScore = highestScore,
                LowestScore = lowestScore,
                QuizResults = quizDetails.OrderByDescending(q => q.TakenAt).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building quiz statistics");
            return new QuizStatisticsDto();
        }
    }

    private async Task<List<TopicProgressDetailDto>> BuildTopicProgressAsync(
        Guid studentId, Guid courseId)
    {
        try
        {
            var topics = await _unitOfWork.TopicRepository
                .GetByCourseIdAsync(courseId) ?? Enumerable.Empty<Topic>();
            var assignments = await _unitOfWork.AssignmentRepository
                .GetByCourseIdAsync(courseId) ?? Enumerable.Empty<Assignment>();
            var solutions = await _unitOfWork.SolutionRepository
                .GetByUserAndCourseAsync(studentId, courseId) ?? Enumerable.Empty<Solution>();

            var assignmentsList = assignments.ToList();
            var solutionsList = solutions.ToList();

            return topics.Select(topic =>
            {
                var topicAssignments = assignmentsList.Where(a => a.TopicId == topic.Id).ToList();
                var completedAssignments = solutionsList
                    .Count(s => topicAssignments.Any(a => a.Id == s.AssignmentId));

                var topicGrades = solutionsList
                    .Where(s => topicAssignments.Any(a => a.Id == s.AssignmentId) && 
                               !string.IsNullOrEmpty(s.Grade))
                    .Select(s => decimal.TryParse(s.Grade, out var g) ? g : 0)
                    .Where(g => g > 0)
                    .ToList();

                return new TopicProgressDetailDto
                {
                    TopicId = topic.Id,
                    TopicName = topic.Name,
                    TotalAssignments = topicAssignments.Count,
                    CompletedAssignments = completedAssignments,
                    AverageGrade = topicGrades.Any() ? Math.Round(topicGrades.Average(), 2) : 0,
                    CompletionRate = topicAssignments.Any() 
                        ? Math.Round((decimal)completedAssignments / topicAssignments.Count * 100, 2) 
                        : 0
                };
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building topic progress");
            return new List<TopicProgressDetailDto>();
        }
    }

    private async Task<List<ModuleProgressDetailDto>> BuildModuleProgressAsync(
        Guid studentId, Guid courseId)
    {
        try
        {
            var modules = await _unitOfWork.ModuleRepository.GetByCourseIdAsync(courseId);
            
            return modules.Select(module => new ModuleProgressDetailDto
            {
                ModuleId = module.Id,
                ModuleName = module.Name,
                IsCompleted = false // Implement your completion logic
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building module progress");
            return new List<ModuleProgressDetailDto>();
        }
    }

    private async Task<CoursePerformanceDto> BuildCoursePerformanceAsync(
        Guid studentId, Guid courseId)
    {
        try
        {
            var solutions = await _unitOfWork.SolutionRepository
                .GetByUserAndCourseAsync(studentId, courseId) ?? Enumerable.Empty<Solution>();
            var examResults = await _unitOfWork.ExamResultRepository
                .GetByUserAndCourseAsync(studentId, courseId) ?? Enumerable.Empty<ExamResult>();

            var solutionsList = solutions.ToList();
            var examResultsList = examResults.ToList();

            var allGrades = new List<decimal>();

            foreach (var solution in solutionsList.Where(s => !string.IsNullOrEmpty(s.Grade)))
            {
                if (decimal.TryParse(solution.Grade, out var grade))
                    allGrades.Add(grade);
            }

            allGrades.AddRange(examResultsList.Select(e => e.Score));

            var overallGrade = allGrades.Any() ? Math.Round(allGrades.Average(), 2) : 0;
            var trend = AnalyzeTrend(solutionsList, examResultsList);

            return new CoursePerformanceDto
            {
                OverallGrade = overallGrade,
                PerformanceTrend = trend,
                TotalActivities = solutionsList.Count + examResultsList.Count,
                LastActivityDate = GetLastActivity(solutionsList, examResultsList)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building course performance");
            return new CoursePerformanceDto();
        }
    }

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

    private string AnalyzeTrend(IEnumerable<Solution> solutions, IEnumerable<ExamResult> examResults)
    {
        var recentDate = DateTimeOffset.UtcNow.AddDays(-30);
        var recentGrades = new List<decimal>();
        var olderGrades = new List<decimal>();

        foreach (var solution in solutions.Where(s => !string.IsNullOrEmpty(s.Grade)))
        {
            if (decimal.TryParse(solution.Grade, out var grade))
            {
                if (solution.CreateDate >= recentDate)
                    recentGrades.Add(grade);
                else
                    olderGrades.Add(grade);
            }
        }

        foreach (var exam in examResults)
        {
            if (exam.FinishedAt >= recentDate)
                recentGrades.Add(exam.Score);
            else
                olderGrades.Add(exam.Score);
        }

        if (!recentGrades.Any() || !olderGrades.Any())
            return "Stable";

        var recentAvg = recentGrades.Average();
        var olderAvg = olderGrades.Average();

        if (recentAvg > olderAvg + 5)
            return "Improving";
        else if (recentAvg < olderAvg - 5)
            return "Declining";
        else
            return "Stable";
    }

    private DateTimeOffset GetLastActivity(IEnumerable<Solution> solutions, IEnumerable<ExamResult> examResults)
    {
        var dates = new List<DateTimeOffset>();

        var solutionsList = solutions.ToList();
        if (solutionsList.Any())
            dates.Add(solutionsList.Max(s => s.CreateDate));

        var examResultsList = examResults.ToList();
        if (examResultsList.Any())
            dates.Add(examResultsList.Max(e => e.FinishedAt));

        return dates.Any() ? dates.Max() : DateTimeOffset.MinValue;
    }
}


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
    public Dictionary<string, int> SubmissionsByType { get; set; }
    public Dictionary<string, decimal> AverageGradeByType { get; set; }
    public int OnTimeSubmissions { get; set; }
    public int LateSubmissions { get; set; }
    public List<AssignmentSubmissionDetailDto> Submissions { get; set; }
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
    public List<QuizResultDetailDto> QuizResults { get; set; }
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
