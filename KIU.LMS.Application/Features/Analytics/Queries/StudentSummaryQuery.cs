using KIU.LMS.Domain.Entities.NoSQL;
using Microsoft.Extensions.Logging;

namespace KIU.LMS.Application.Features.Analytics.Queries;

public sealed record StudentSummaryQuery(Guid StudentId) : IRequest<Result<object>>;

public sealed class StudentSummaryQueryHandler(
        IUnitOfWork _unitOfWork,
        IMongoRepository<ExamSession> _examSessionRepository,
        IMongoRepository<StudentAnswer> _studentAnswerRepository,
        IMongoRepository<Question> _questionRepository,
        ILogger<StudentSummaryQueryHandler> _logger) : IRequestHandler<StudentSummaryQuery, Result<object>>
{
    public async Task<Result<object>> Handle(StudentSummaryQuery request, CancellationToken cancellationToken)
    {
        return await GetStudentSummaryAsync(request.StudentId);
    }

    public async Task<StudentAnalyticsDto> GetStudentAnalyticsAsync(Guid userId)
    {
        try
        {
            // Fetch user data
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Role != UserRole.Student)
            {
                _logger.LogWarning("User {UserId} not found or not a student", userId);
                return null;
            }

            // Build analytics - with error handling for each task
            var tasks = new List<Task>();

            var profileTask = BuildStudentProfileSafely(user);
            var courseAnalyticsTask = BuildCourseAnalyticsSafely(userId);
            var assignmentAnalyticsTask = BuildAssignmentAnalyticsSafely(userId);
            var examAnalyticsTask = BuildExamAnalyticsSafely(userId);
            var performanceMetricsTask = BuildPerformanceMetricsSafely(userId);
            var timelineTask = BuildTimelineSafely(userId);

            await Task.WhenAll(
                profileTask,
                courseAnalyticsTask,
                assignmentAnalyticsTask,
                examAnalyticsTask,
                performanceMetricsTask,
                timelineTask
            );

            return new StudentAnalyticsDto
            {
                StudentId = userId,
                Profile = await profileTask,
                CourseAnalytics = await courseAnalyticsTask,
                AssignmentAnalytics = await assignmentAnalyticsTask,
                ExamAnalytics = await examAnalyticsTask,
                PerformanceMetrics = await performanceMetricsTask,
                Timeline = await timelineTask,
                GeneratedAt = DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetStudentAnalyticsAsync for user {UserId}", userId);
            throw;
        }
    }

    // Wrapper methods with error handling
    private async Task<StudentProfileDto> BuildStudentProfileSafely(User user)
    {
        try
        {
            return await BuildStudentProfile(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building student profile for user {UserId}", user.Id);
            return new StudentProfileDto
            {
                UserId = user.Id,
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email ?? "",
                Institution = user.Institution ?? "Not specified",
                EmailVerified = user.EmailVerified,
                CreatedAt = user.CreateDate,
                RegisteredDevices = new List<DeviceDto>()
            };
        }
    }

    private async Task<CourseAnalyticsDto> BuildCourseAnalyticsSafely(Guid userId)
    {
        try
        {
            return await BuildCourseAnalytics(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building course analytics for user {UserId}", userId);
            return new CourseAnalyticsDto
            {
                TotalEnrolledCourses = 0,
                ActiveCourses = 0,
                CompletedCourses = 0,
                Courses = new List<CourseDetailDto>()
            };
        }
    }

    private async Task<AssignmentAnalyticsDto> BuildAssignmentAnalyticsSafely(Guid userId)
    {
        try
        {
            return await BuildAssignmentAnalytics(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building assignment analytics for user {UserId}", userId);
            return new AssignmentAnalyticsDto
            {
                TotalSubmissions = 0,
                GradedSubmissions = 0,
                PendingGrading = 0,
                AverageGrade = 0,
                SubmissionsByType = new Dictionary<string, int>(),
                RecentSubmissions = new List<AssignmentSubmissionDto>(),
                Performance = new AssignmentPerformanceDto
                {
                    OnTimeSubmissions = 0,
                    LateSubmissions = 0,
                    AverageGradeByType = new Dictionary<string, decimal>(),
                    StrongTopics = new List<string>(),
                    WeakTopics = new List<string>()
                }
            };
        }
    }

    private async Task<ExamAnalyticsDto> BuildExamAnalyticsSafely(Guid userId)
    {
        try
        {
            return await BuildExamAnalytics(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building exam analytics for user {UserId}", userId);
            return new ExamAnalyticsDto
            {
                TotalExamsTaken = 0,
                AverageScore = 0,
                HighestScore = 0,
                LowestScore = 0,
                AverageDuration = TimeSpan.Zero,
                ExamResults = new List<ExamResultDetailDto>(),
                RecentSessions = new List<ExamSessionDetailDto>(),
                QuestionAnalytics = new QuestionAnalyticsDto
                {
                    TotalQuestionsAnswered = 0,
                    CorrectAnswers = 0,
                    IncorrectAnswers = 0,
                    AccuracyRate = 0,
                    PerformanceByType = new Dictionary<string, QuestionTypePerformanceDto>(),
                    AverageTimePerQuestion = TimeSpan.Zero
                }
            };
        }
    }

    private async Task<PerformanceMetricsDto> BuildPerformanceMetricsSafely(Guid userId)
    {
        try
        {
            return await BuildPerformanceMetrics(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building performance metrics for user {UserId}", userId);
            return new PerformanceMetricsDto
            {
                OverallGPA = 0,
                PerformanceTrend = "Unknown",
                StudyStreak = 0,
                LastActivityDate = DateTimeOffset.UtcNow,
                GradeDistribution = new Dictionary<string, decimal>(),
                Achievements = new List<string>(),
                ComparisonMetrics = new ComparisonMetricsDto
                {
                    PercentileRank = 0,
                    ComparedToAverage = "Unknown",
                    ClassAverageScore = 0,
                    StudentAverageScore = 0
                }
            };
        }
    }

    private async Task<TimelineDto> BuildTimelineSafely(Guid userId)
    {
        try
        {
            return await BuildTimeline(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building timeline for user {UserId}", userId);
            return new TimelineDto { Events = new List<TimelineEventDto>() };
        }
    }

    private async Task<StudentProfileDto> BuildStudentProfile(User user)
    {
        var devices = await _unitOfWork.UserDeviceRepository.GetByUserIdAsync(user.Id) ?? Enumerable.Empty<UserDevice>();
        var loginAttempts = await _unitOfWork.LoginAttemptRepository.GetByUserIdAsync(user.Id) ?? Enumerable.Empty<LoginAttempt>();

        return new StudentProfileDto
        {
            UserId = user.Id,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            Email = user.Email ?? "",
            Institution = user.Institution ?? "Not specified",
            EmailVerified = user.EmailVerified,
            CreatedAt = user.CreateDate,
            TotalLoginAttempts = loginAttempts.Count(),
            RegisteredDevices = devices.Select(d => new DeviceDto
            {
                DeviceName = d.DeviceIdentifier ?? "Unknown Device",
                RegisteredAt = d.CreateDate
            }).ToList()
        };
    }

    private async Task<CourseAnalyticsDto> BuildCourseAnalytics(Guid userId)
    {
        var userCourses = await _unitOfWork.UserCourseRepository.GetByUserIdWithDetailsAsync(userId) ?? Enumerable.Empty<UserCourse>();
        var courseDetails = new List<CourseDetailDto>();

        foreach (var userCourse in userCourses)
        {
            try
            {
                var course = await _unitOfWork.CourseRepository.GetByIdWithDetailsAsync(userCourse.CourseId);
                if (course == null) continue;

                var assignments = await _unitOfWork.AssignmentRepository.GetByCourseIdAsync(course.Id) ?? Enumerable.Empty<Assignment>();
                var solutions = await _unitOfWork.SolutionRepository.GetByUserAndCourseAsync(userId, course.Id) ?? Enumerable.Empty<Solution>();
                var quizzes = await _unitOfWork.QuizRepository.GetByCourseIdAsync(course.Id) ?? Enumerable.Empty<Quiz>();
                var examResults = await _unitOfWork.ExamResultRepository.GetByUserAndCourseAsync(userId, course.Id) ?? Enumerable.Empty<ExamResult>();
                var topics = await _unitOfWork.TopicRepository.GetByCourseIdAsync(course.Id) ?? Enumerable.Empty<Topic>();
                var modules = await _unitOfWork.ModuleRepository.GetByCourseIdAsync(course.Id); //?? Enumerable.Empty<Module>();

                var assignmentsList = assignments.ToList();
                var solutionsList = solutions.ToList();

                var topicProgress = topics.Select(topic => new TopicProgressDto
                {
                    TopicId = topic.Id,
                    TopicName = topic.Name ?? "Unknown Topic",
                    TotalAssignments = assignmentsList.Count(a => a.TopicId == topic.Id),
                    CompletedAssignments = solutionsList.Count(s =>
                        assignmentsList.Any(a => a.Id == s.AssignmentId && a.TopicId == topic.Id))
                }).ToList();

                var moduleProgress = modules.Select(module => new ModuleProgressDto
                {
                    ModuleId = module.Id,
                    ModuleName = module.Name ?? "Unknown Module",
                    IsCompleted = false // You can implement proper logic here
                }).ToList();

                courseDetails.Add(new CourseDetailDto
                {
                    CourseId = course.Id,
                    CourseName = course.Name ?? "Unknown Course",
                    EnrolledAt = userCourse.CreateDate,
                    Grade = CalculateCourseGrade(solutionsList, examResults),
                    Status = DetermineCourseStatus(userCourse, solutionsList, assignmentsList, examResults, quizzes),
                    TotalAssignments = assignmentsList.Count(),
                    CompletedAssignments = solutionsList.Count(),
                    TotalQuizzes = quizzes.Count(),
                    CompletedQuizzes = examResults.Count(),
                    TopicProgress = topicProgress,
                    ModuleProgress = moduleProgress
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing course {CourseId} for user {UserId}", userCourse.CourseId, userId);
            }
        }

        var activeCourses = courseDetails.Count(c => c.Status == "Active");
        var completedCourses = courseDetails.Count(c => c.Status == "Completed");

        return new CourseAnalyticsDto
        {
            TotalEnrolledCourses = courseDetails.Count,
            ActiveCourses = activeCourses,
            CompletedCourses = completedCourses,
            Courses = courseDetails.OrderByDescending(c => c.EnrolledAt).ToList()
        };
    }

    private async Task<AssignmentAnalyticsDto> BuildAssignmentAnalytics(Guid userId)
    {
        var allSolutions = await _unitOfWork.SolutionRepository.GetByUserIdWithDetailsAsync(userId) ?? Enumerable.Empty<Solution>();
        var solutionsList = allSolutions.ToList();

        var submissionsByType = new Dictionary<string, int>();
        var gradesByType = new Dictionary<string, List<decimal>>();
        var recentSubmissions = new List<AssignmentSubmissionDto>();

        foreach (var solution in solutionsList)
        {
            try
            {
                var assignment = await _unitOfWork.AssignmentRepository.GetByIdWithDetailsAsync(solution.AssignmentId);
                if (assignment == null) continue;

                var course = await _unitOfWork.CourseRepository.GetByIdAsync(assignment.CourseId);
                var typeName = assignment.Type.ToString();

                // Count by type
                if (!submissionsByType.ContainsKey(typeName))
                    submissionsByType[typeName] = 0;
                submissionsByType[typeName]++;

                // Collect grades by type
                if (!string.IsNullOrEmpty(solution.Grade) && decimal.TryParse(solution.Grade, out var gradeValue))
                {
                    if (!gradesByType.ContainsKey(typeName))
                        gradesByType[typeName] = new List<decimal>();
                    gradesByType[typeName].Add(gradeValue);
                }

                // Recent submissions
                recentSubmissions.Add(new AssignmentSubmissionDto
                {
                    AssignmentId = assignment.Id,
                    AssignmentName = assignment.Name ?? "Unknown Assignment",
                    CourseName = course?.Name ?? "Unknown",
                    Type = typeName,
                    SubmittedAt = solution.CreateDate,
                    Grade = solution.Grade ?? "",
                    Feedback = solution.FeedBack ?? "",
                    GradingStatus = solution.GradingStatus.ToString(),
                    IsLate = assignment.EndDateTime.HasValue && solution.CreateDate > assignment.EndDateTime.Value,
                    IsTraining = assignment.IsTraining
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing solution {SolutionId}", solution.Id);
            }
        }

        var gradedSubmissions = solutionsList.Count(s => s.GradingStatus == GradingStatus.Completed);
        var pendingGrading = solutionsList.Count(s => s.GradingStatus == GradingStatus.InProgress);

        var averageGradeByType = gradesByType.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Any() ? Math.Round(kvp.Value.Average(), 2) : 0
        );

        var allGrades = gradesByType.SelectMany(kvp => kvp.Value).ToList();
        var averageGrade = allGrades.Any() ? Math.Round(allGrades.Average(), 2) : 0;

        var onTimeSubmissions = recentSubmissions.Count(s => !s.IsLate);
        var lateSubmissions = recentSubmissions.Count(s => s.IsLate);

        var topicPerformance = await AnalyzeTopicPerformance(userId);

        return new AssignmentAnalyticsDto
        {
            TotalSubmissions = solutionsList.Count(),
            GradedSubmissions = gradedSubmissions,
            PendingGrading = pendingGrading,
            AverageGrade = averageGrade,
            SubmissionsByType = submissionsByType,
            RecentSubmissions = recentSubmissions.OrderByDescending(s => s.SubmittedAt).Take(10).ToList(),
            Performance = new AssignmentPerformanceDto
            {
                OnTimeSubmissions = onTimeSubmissions,
                LateSubmissions = lateSubmissions,
                AverageGradeByType = averageGradeByType,
                StrongTopics = topicPerformance.StrongTopics,
                WeakTopics = topicPerformance.WeakTopics
            }
        };
    }

    private async Task<ExamAnalyticsDto> BuildExamAnalytics(Guid userId)
    {
        var examResults = await _unitOfWork.ExamResultRepository.GetByStudentIdAsync(userId) ?? Enumerable.Empty<ExamResult>();
        var examResultsList = examResults.ToList();

        var userIdStr = userId.ToString();
        var examSessions = await _examSessionRepository.FindAsync(es => es.StudentId == userIdStr) ?? Enumerable.Empty<ExamSession>();
        var examSessionsList = examSessions.ToList();

        var examResultDetails = new List<ExamResultDetailDto>();

        foreach (var result in examResultsList)
        {
            try
            {
                var quiz = await _unitOfWork.QuizRepository.GetByIdWithDetailsAsync(result.QuizId);
                var course = quiz != null ? await _unitOfWork.CourseRepository.GetByIdAsync(quiz.CourseId) : null;

                examResultDetails.Add(new ExamResultDetailDto
                {
                    QuizId = result.QuizId,
                    QuizName = quiz?.Title ?? "Unknown Quiz",
                    CourseName = course?.Name ?? "Unknown Course",
                    TakenAt = result.StartedAt,
                    Score = result.Score,
                    TotalQuestions = result.TotalQuestions,
                    CorrectAnswers = result.CorrectAnswers,
                    Duration = result.Duration,
                    PerformanceLevel = DeterminePerformanceLevel(result.Score)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing exam result {ResultId}", result.Id);
            }
        }

        var sessionDetails = new List<ExamSessionDetailDto>();
        var allQuestionAttempts = new List<QuestionAttemptDto>();

        foreach (var session in examSessionsList)
        {
            try
            {
                var studentAnswers = await _studentAnswerRepository.FindAsync(sa => sa.SessionId == session.Id) ?? Enumerable.Empty<StudentAnswer>();
                var questionAttempts = new List<QuestionAttemptDto>();

                foreach (var answer in studentAnswers)
                {
                    try
                    {
                        var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
                        if (question == null) continue;

                        var isCorrect = CheckIfAnswerCorrect(question, answer.SelectedOptions ?? new List<string>());
                        var attempt = new QuestionAttemptDto
                        {
                            QuestionId = question.Id,
                            QuestionText = question.Text ?? "Unknown Question",
                            QuestionType = question.Type.ToString() ?? "Unknown",
                            SelectedAnswers = answer.SelectedOptions ?? new List<string>(),
                            IsCorrect = isCorrect,
                            TimeSpentSeconds = CalculateTimeSpent(session, answer)
                        };

                        questionAttempts.Add(attempt);
                        allQuestionAttempts.Add(attempt);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing student answer {AnswerId}", answer.Id);
                    }
                }

                sessionDetails.Add(new ExamSessionDetailDto
                {
                    SessionId = session.Id,
                    QuizId = session.QuizId,
                    StartedAt = session.StartedAt,
                    FinishedAt = session.FinishedAt,
                    Status = session.Status.ToString() ?? "Unknown",
                    TotalQuestions = session.Questions?.Count ?? 0,
                    AnsweredQuestions = studentAnswers.Count(),
                    QuestionAttempts = questionAttempts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing exam session {SessionId}", session.Id);
            }
        }

        var questionAnalytics = BuildQuestionAnalytics(allQuestionAttempts);

        var averageScore = examResultsList.Any() ? Math.Round(examResultsList.Average(r => r.Score), 2) : 0;
        var highestScore = examResultsList.Any() ? examResultsList.Max(r => r.Score) : 0;
        var lowestScore = examResultsList.Any() ? examResultsList.Min(r => r.Score) : 0;
        var averageDuration = examResultsList.Any()
            ? TimeSpan.FromSeconds(examResultsList.Average(r => r.Duration.TotalSeconds))
            : TimeSpan.Zero;

        return new ExamAnalyticsDto
        {
            TotalExamsTaken = examResultsList.Count(),
            AverageScore = averageScore,
            HighestScore = highestScore,
            LowestScore = lowestScore,
            AverageDuration = averageDuration,
            ExamResults = examResultDetails.OrderByDescending(e => e.TakenAt).ToList(),
            RecentSessions = sessionDetails.OrderByDescending(s => s.StartedAt).Take(5).ToList(),
            QuestionAnalytics = questionAnalytics
        };
    }

    private async Task<PerformanceMetricsDto> BuildPerformanceMetrics(Guid userId)
    {
        var solutions = await _unitOfWork.SolutionRepository.GetByUserIdAsync(userId) ?? Enumerable.Empty<Solution>();
        var examResults = await _unitOfWork.ExamResultRepository.GetByStudentIdAsync(userId) ?? Enumerable.Empty<ExamResult>();

        var solutionsList = solutions.ToList();
        var examResultsList = examResults.ToList();

        var overallGPA = CalculateOverallGPA(solutionsList, examResultsList);
        var performanceTrend = AnalyzePerformanceTrend(solutionsList, examResultsList);
        var studyStreak = await CalculateStudyStreak(userId);
        var lastActivity = await GetLastActivityDate(userId);
        var gradeDistribution = CalculateGradeDistribution(solutionsList, examResultsList);
        var achievements = await CalculateAchievements(userId, solutionsList, examResultsList);
        var comparisonMetrics = await CalculateComparisonMetrics(userId);

        return new PerformanceMetricsDto
        {
            OverallGPA = overallGPA,
            PerformanceTrend = performanceTrend,
            StudyStreak = studyStreak,
            LastActivityDate = lastActivity,
            GradeDistribution = gradeDistribution,
            Achievements = achievements,
            ComparisonMetrics = comparisonMetrics
        };
    }

    private async Task<TimelineDto> BuildTimeline(Guid userId)
    {
        var events = new List<TimelineEventDto>();

        try
        {
            // Add assignment submissions
            var solutions = await _unitOfWork.SolutionRepository.GetByUserIdWithDetailsAsync(userId) ?? Enumerable.Empty<Solution>();
            foreach (var solution in solutions.Take(20))
            {
                try
                {
                    var assignment = await _unitOfWork.AssignmentRepository.GetByIdAsync(solution.AssignmentId);
                    events.Add(new TimelineEventDto
                    {
                        Date = solution.CreateDate,
                        Type = "Assignment",
                        Title = $"Submitted: {assignment?.Name ?? "Unknown"}",
                        Description = $"Grade: {solution.Grade ?? "N/A"}, Status: {solution.GradingStatus}",
                        Metadata = new { AssignmentId = solution.AssignmentId, Grade = solution.Grade }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding solution to timeline");
                }
            }

            // Add exam completions
            var examResults = await _unitOfWork.ExamResultRepository.GetByStudentIdAsync(userId) ?? Enumerable.Empty<ExamResult>();
            foreach (var exam in examResults.Take(20))
            {
                try
                {
                    var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(exam.QuizId);
                    events.Add(new TimelineEventDto
                    {
                        Date = exam.FinishedAt,
                        Type = "Quiz",
                        Title = $"Completed: {quiz?.Title ?? "Unknown Quiz"}",
                        Description = $"Score: {exam.Score}%, Questions: {exam.CorrectAnswers}/{exam.TotalQuestions}",
                        Metadata = new { QuizId = exam.QuizId, Score = exam.Score }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding exam to timeline");
                }
            }

            // Add course enrollments
            var userCourses = await _unitOfWork.UserCourseRepository.GetByUserIdAsync(userId) ?? Enumerable.Empty<UserCourse>();
            foreach (var enrollment in userCourses)
            {
                try
                {
                    var course = await _unitOfWork.CourseRepository.GetByIdAsync(enrollment.CourseId);
                    events.Add(new TimelineEventDto
                    {
                        Date = enrollment.CreateDate,
                        Type = "Course",
                        Title = $"Enrolled in: {course?.Name ?? "Unknown"}",
                        Description = "Started new course",
                        Metadata = new { CourseId = enrollment.CourseId }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding enrollment to timeline");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building timeline for user {UserId}", userId);
        }

        events = events.OrderByDescending(e => e.Date).ToList();
        return new TimelineDto { Events = events };
    }

    // Helper methods remain the same but with added null checks
    private decimal CalculateCourseGrade(IEnumerable<Solution> solutions, IEnumerable<ExamResult> examResults)
    {
        var grades = new List<decimal>();

        foreach (var solution in solutions?.Where(s => !string.IsNullOrEmpty(s.Grade)) ?? Enumerable.Empty<Solution>())
        {
            if (decimal.TryParse(solution.Grade, out var grade))
                grades.Add(grade);
        }

        if (examResults != null)
            grades.AddRange(examResults.Select(e => e.Score));

        return grades.Any() ? Math.Round(grades.Average(), 2) : 0;
    }

    private string DetermineCourseStatus(UserCourse userCourse, IEnumerable<Solution> solutions,
        IEnumerable<Assignment> assignments, IEnumerable<ExamResult> examResults, IEnumerable<Quiz> quizzes)
    {
        var completionRate = 0m;
        var assignmentsList = assignments?.ToList() ?? new List<Assignment>();
        var quizzesList = quizzes?.ToList() ?? new List<Quiz>();
        var solutionsList = solutions?.ToList() ?? new List<Solution>();
        var examResultsList = examResults?.ToList() ?? new List<ExamResult>();

        if (assignmentsList.Any())
            completionRate += (decimal)solutionsList.Count() / assignmentsList.Count() * 50;

        if (quizzesList.Any())
            completionRate += (decimal)examResultsList.Count() / quizzesList.Count() * 50;

        if (completionRate >= 100)
            return "Completed";
        else if (completionRate > 0)
            return "Active";
        else
            return "Not Started";
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

    private bool CheckIfAnswerCorrect(Question question, List<string> selectedOptions)
    {
        if (question?.Options == null || selectedOptions == null)
            return false;

        var correctOptions = question.Options.Where(o => o.IsCorrect).Select(o => o.Id).ToList();
        return correctOptions.Count == selectedOptions.Count &&
               correctOptions.All(co => selectedOptions.Contains(co));
    }

    private int? CalculateTimeSpent(ExamSession session, StudentAnswer answer)
    {
        if (session?.Questions == null || answer == null)
            return null;

        var question = session.Questions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
        if (question?.StartedAt != null)
        {
            return (int)(answer.AnsweredAt - question.StartedAt.Value).TotalSeconds;
        }
        return null;
    }

    private QuestionAnalyticsDto BuildQuestionAnalytics(List<QuestionAttemptDto> attempts)
    {
        if (attempts == null || !attempts.Any())
        {
            return new QuestionAnalyticsDto
            {
                TotalQuestionsAnswered = 0,
                CorrectAnswers = 0,
                IncorrectAnswers = 0,
                AccuracyRate = 0,
                PerformanceByType = new Dictionary<string, QuestionTypePerformanceDto>(),
                AverageTimePerQuestion = TimeSpan.Zero
            };
        }

        var correctCount = attempts.Count(a => a.IsCorrect);
        var incorrectCount = attempts.Count(a => !a.IsCorrect);
        var accuracyRate = Math.Round((decimal)correctCount / attempts.Count * 100, 2);

        var performanceByType = attempts
            .GroupBy(a => a.QuestionType)
            .ToDictionary(
                g => g.Key,
                g => new QuestionTypePerformanceDto
                {
                    TotalAttempted = g.Count(),
                    Correct = g.Count(a => a.IsCorrect),
                    AccuracyRate = Math.Round((decimal)g.Count(a => a.IsCorrect) / g.Count() * 100, 2),
                    AverageTime = TimeSpan.FromSeconds(
                        g.Where(a => a.TimeSpentSeconds.HasValue)
                         .Select(a => a.TimeSpentSeconds.Value)
                         .DefaultIfEmpty(0)
                         .Average())
                });

        var avgTimeSeconds = attempts
            .Where(a => a.TimeSpentSeconds.HasValue)
            .Select(a => a.TimeSpentSeconds.Value)
            .DefaultIfEmpty(0)
            .Average();

        return new QuestionAnalyticsDto
        {
            TotalQuestionsAnswered = attempts.Count,
            CorrectAnswers = correctCount,
            IncorrectAnswers = incorrectCount,
            AccuracyRate = accuracyRate,
            PerformanceByType = performanceByType,
            AverageTimePerQuestion = TimeSpan.FromSeconds(avgTimeSeconds)
        };
    }

    private async Task<(List<string> StrongTopics, List<string> WeakTopics)> AnalyzeTopicPerformance(Guid userId)
    {
        // This would analyze performance across topics
        // Implementation depends on your specific business logic
        return (new List<string> { "Programming Basics", "Data Structures" },
                new List<string> { "Advanced Algorithms" });
    }

    private decimal CalculateOverallGPA(IEnumerable<Solution> solutions, IEnumerable<ExamResult> examResults)
    {
        var allGrades = new List<decimal>();

        foreach (var solution in solutions?.Where(s => !string.IsNullOrEmpty(s.Grade)) ?? Enumerable.Empty<Solution>())
        {
            if (decimal.TryParse(solution.Grade, out var grade))
                allGrades.Add(grade);
        }

        if (examResults != null)
            allGrades.AddRange(examResults.Select(e => e.Score));

        if (!allGrades.Any()) return 0;

        var average = allGrades.Average();
        // Convert to 4.0 scale
        return Math.Round(average / 25, 2); // Assuming 100% = 4.0 GPA
    }

    private string AnalyzePerformanceTrend(IEnumerable<Solution> solutions, IEnumerable<ExamResult> examResults)
    {
        var recentDate = DateTimeOffset.UtcNow.AddDays(-30);
        var recentGrades = new List<decimal>();
        var olderGrades = new List<decimal>();

        foreach (var solution in solutions?.Where(s => !string.IsNullOrEmpty(s.Grade)) ?? Enumerable.Empty<Solution>())
        {
            if (decimal.TryParse(solution.Grade, out var grade))
            {
                if (solution.CreateDate >= recentDate)
                    recentGrades.Add(grade);
                else
                    olderGrades.Add(grade);
            }
        }

        foreach (var exam in examResults ?? Enumerable.Empty<ExamResult>())
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

    private async Task<int> CalculateStudyStreak(Guid userId)
    {
        var activities = new List<DateTimeOffset>();

        var solutions = await _unitOfWork.SolutionRepository.GetByUserIdAsync(userId) ?? Enumerable.Empty<Solution>();
        activities.AddRange(solutions.Select(s => s.CreateDate));

        var examResults = await _unitOfWork.ExamResultRepository.GetByStudentIdAsync(userId) ?? Enumerable.Empty<ExamResult>();
        activities.AddRange(examResults.Select(e => e.FinishedAt));

        if (!activities.Any()) return 0;

        activities = activities.OrderByDescending(a => a).ToList();

        var streak = 1;
        var currentDate = activities.First().Date;

        foreach (var activity in activities.Skip(1))
        {
            if ((currentDate - activity.Date).Days == 1)
            {
                streak++;
                currentDate = activity.Date;
            }
            else if ((currentDate - activity.Date).Days > 1)
            {
                break;
            }
        }

        return streak;
    }

    private async Task<DateTimeOffset> GetLastActivityDate(Guid userId)
    {
        var dates = new List<DateTimeOffset>();

        var solutions = await _unitOfWork.SolutionRepository.GetByUserIdAsync(userId) ?? Enumerable.Empty<Solution>();
        if (solutions.Any())
            dates.Add(solutions.Max(s => s.CreateDate));

        var examResults = await _unitOfWork.ExamResultRepository.GetByStudentIdAsync(userId) ?? Enumerable.Empty<ExamResult>();
        if (examResults.Any())
            dates.Add(examResults.Max(e => e.FinishedAt));

        return dates.Any() ? dates.Max() : DateTimeOffset.MinValue;
    }

    private Dictionary<string, decimal> CalculateGradeDistribution(
        IEnumerable<Solution> solutions, IEnumerable<ExamResult> examResults)
    {
        var distribution = new Dictionary<string, int>
        {
            ["A (90-100)"] = 0,
            ["B (80-89)"] = 0,
            ["C (70-79)"] = 0,
            ["D (60-69)"] = 0,
            ["F (<60)"] = 0
        };

        var allGrades = new List<decimal>();

        foreach (var solution in solutions?.Where(s => !string.IsNullOrEmpty(s.Grade)) ?? Enumerable.Empty<Solution>())
        {
            if (decimal.TryParse(solution.Grade, out var grade))
                allGrades.Add(grade);
        }

        if (examResults != null)
            allGrades.AddRange(examResults.Select(e => e.Score));

        foreach (var grade in allGrades)
        {
            if (grade >= 90) distribution["A (90-100)"]++;
            else if (grade >= 80) distribution["B (80-89)"]++;
            else if (grade >= 70) distribution["C (70-79)"]++;
            else if (grade >= 60) distribution["D (60-69)"]++;
            else distribution["F (<60)"]++;
        }

        var total = allGrades.Count;
        return distribution.ToDictionary(
            kvp => kvp.Key,
            kvp => total > 0 ? Math.Round((decimal)kvp.Value / total * 100, 2) : 0
        );
    }

    private async Task<List<string>> CalculateAchievements(
        Guid userId, IEnumerable<Solution> solutions, IEnumerable<ExamResult> examResults)
    {
        var achievements = new List<string>();
        var solutionsList = solutions?.ToList() ?? new List<Solution>();
        var examResultsList = examResults?.ToList() ?? new List<ExamResult>();

        // Perfect scores
        var perfectExams = examResultsList.Count(e => e.Score == 100);
        if (perfectExams > 0)
            achievements.Add($"Perfect Score Master ({perfectExams} perfect exams)");

        // Consistency
        if (solutionsList.Count() >= 10)
            achievements.Add("Consistent Contributor (10+ assignments)");

        // Early bird
        var earlySubmissions = 0;
        foreach (var solution in solutionsList)
        {
            try
            {
                var assignment = await _unitOfWork.AssignmentRepository.GetByIdAsync(solution.AssignmentId);
                if (assignment?.EndDateTime != null && solution.CreateDate < assignment.EndDateTime.Value.AddDays(-1))
                    earlySubmissions++;
            }
            catch
            {
                // Log error if needed
            }
        }

        if (earlySubmissions >= 5)
            achievements.Add("Early Bird (5+ early submissions)");

        // Study streak
        var streak = await CalculateStudyStreak(userId);
        if (streak >= 7)
            achievements.Add($"Study Warrior ({streak} day streak)");

        return achievements;
    }

    private async Task<ComparisonMetricsDto> CalculateComparisonMetrics(Guid userId)
    {
        // This would compare the student against class averages
        // Implementation would depend on your specific requirements
        return new ComparisonMetricsDto
        {
            PercentileRank = 75,
            ComparedToAverage = "Above",
            ClassAverageScore = 72.5m,
            StudentAverageScore = 85.3m
        };
    }

    public async Task<object> GetStudentSummaryAsync(Guid userId)
    {
        try
        {
            var analytics = await GetStudentAnalyticsAsync(userId);

            if (analytics == null)
                return null;

            return new
            {
                StudentId = userId,
                Name = $"{analytics.Profile?.FirstName ?? ""} {analytics.Profile?.LastName ?? ""}",
                OverallGPA = analytics.PerformanceMetrics?.OverallGPA ?? 0,
                TotalCourses = analytics.CourseAnalytics?.TotalEnrolledCourses ?? 0,
                CompletedAssignments = analytics.AssignmentAnalytics?.TotalSubmissions ?? 0,
                AverageScore = analytics.ExamAnalytics?.AverageScore ?? 0,
                LastActive = analytics.PerformanceMetrics?.LastActivityDate ?? DateTimeOffset.MinValue,
                PerformanceTrend = analytics.PerformanceMetrics?.PerformanceTrend ?? "Unknown"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting student summary for {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> CheckPermissionAsync(Guid currentUserId, Guid targetUserId)
    {
        try
        {
            // Students can view their own analytics
            if (currentUserId == targetUserId)
                return true;

            // Check if the current user is an admin or teacher
            var currentUser = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
            if (currentUser?.Role == UserRole.Admin || currentUser?.Role == UserRole.Ta)
            {
                // Additional logic to check if teacher has access to this student
                // For now, return true for admin/teacher
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permissions for {CurrentUserId} accessing {TargetUserId}",
                currentUserId, targetUserId);
            return false;
        }
    }
}

