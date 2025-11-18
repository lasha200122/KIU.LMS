
using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCourseStatisticsQuery(Guid Id) : IRequest<Result<GetCourseStatisticsQueryResponse>>;

public sealed record GetCourseStatisticsQueryResponse(
    int TotalModules,
    int TotalBanks, 
    int TotalQuestions, 
    int TotalAssignments, 
    AssignmentStatistic C2RS,
    AssignmentStatistic IPEQ,
    AssignmentStatistic Projects,
    AssignmentStatistic MCQ);

public sealed record AssignmentStatistic(
    int Modules,
    int Banks,
    int Question,
    int Assignments);

public class GetCourseStatisticsQueryHandler : IRequestHandler<GetCourseStatisticsQuery, Result<GetCourseStatisticsQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMongoRepository<Question> _questionRepository;
    
    public GetCourseStatisticsQueryHandler(
        IUnitOfWork unitOfWork,
        IMongoRepository<Question> questionRepository)
    {
        _unitOfWork = unitOfWork;
        _questionRepository = questionRepository;
    }
    
    public async Task<Result<GetCourseStatisticsQueryResponse>> Handle(
        GetCourseStatisticsQuery request, 
        CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdWithDetailsAsync(request.Id);

        if (course is null)
            return Result<GetCourseStatisticsQueryResponse>.Failure("Course not found");

        var c2rsModules = course.Modules
            .SelectMany(m => m.ModuleBanks)
            .Count(mb => mb.Type == SubModuleType.C2RS);

        var ipeqModules = course.Modules
            .SelectMany(m => m.ModuleBanks)
            .Count(mb => mb.Type == SubModuleType.IPEQ);

        var projectsModules = course.Modules
            .SelectMany(m => m.ModuleBanks)
            .Count(mb => mb.Type == SubModuleType.Project);

        var mcqModules = course.Modules
            .SelectMany(m => m.ModuleBanks)
            .Count(mb => mb.Type == SubModuleType.MCQ);

        var c2rsQuizBanks = course.Quizzes
            .Where(quiz => quiz.Type == QuizType.C2RS)
            .SelectMany(q => q.QuizBanks)
            .Count();

        var ipeqQuizBanks = course.Quizzes
            .Where(quiz => quiz.Type == QuizType.IPEQ)
            .SelectMany(q => q.QuizBanks)
            .Count();

        var projectsQuizBanks = course.Quizzes
            .Where(quiz => quiz.Type == QuizType.Projects)
            .SelectMany(q => q.QuizBanks)
            .Count();

        var mcqQuizBanks = course.Quizzes
            .Where(quiz => quiz.Type == QuizType.MCQ)
            .SelectMany(q => q.QuizBanks)
            .Count();

        var c2RsAssignments = course.Assignments.Count(a => a.Type == AssignmentType.C2RS);
        var ipeqAssignments = course.Assignments.Count(a => a.Type == AssignmentType.IPEQ);
        var projectsAssignments = course.Assignments.Count(a => a.Type == AssignmentType.Project);
        var mcqAssignments = course.Assignments.Count(a => a.Type == AssignmentType.MCQ);

        var c2rsQuestions = await GetQuestionCountByModuleBankType(course, SubModuleType.C2RS);
        var ipeqQuestions = await GetQuestionCountByModuleBankType(course, SubModuleType.IPEQ);
        var projectsQuestions = await GetQuestionCountByModuleBankType(course, SubModuleType.Project);
        var mcqQuestions = await GetQuestionCountByModuleBankType(course, SubModuleType.MCQ);

        var totalQuestions = c2rsQuestions + ipeqQuestions + projectsQuestions + mcqQuestions;

        var statsC2Rs = new AssignmentStatistic(c2rsModules, c2rsQuizBanks, c2rsQuestions, c2RsAssignments);
        var statsIpeq = new AssignmentStatistic(ipeqModules, ipeqQuizBanks, ipeqQuestions, ipeqAssignments);
        var statsProjects = new AssignmentStatistic(projectsModules, projectsQuizBanks, projectsQuestions, projectsAssignments);
        var statsMcq = new AssignmentStatistic(mcqModules, mcqQuizBanks, mcqQuestions, mcqAssignments);

        var result = new GetCourseStatisticsQueryResponse(
            course.Modules.Count,
            course.Quizzes.SelectMany(q => q.QuizBanks).Count(),
            totalQuestions,
            course.Assignments.Count,
            statsC2Rs,
            statsIpeq, 
            statsProjects, 
            statsMcq
        );

        return Result<GetCourseStatisticsQueryResponse>.Success(result);
    }

    private async Task<int> GetQuestionCountByModuleBankType(Course course, SubModuleType type)
    {
        var questionBankIds = course.Modules
            .SelectMany(m => m.ModuleBanks)
            .Where(mb => mb.Type == type)
            .SelectMany(mb => mb.Module.QuestionBanks)
            .Select(qb => qb.Id.ToString())
            .Distinct()
            .ToList();

        if (!questionBankIds.Any())
            return 0;

        var questions = await _questionRepository.FindAsync(q => questionBankIds.Contains(q.QuestionBankId));
        return questions.Count();
    }

}
