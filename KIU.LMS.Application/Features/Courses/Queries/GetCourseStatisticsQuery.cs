
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

public class GetCourseStatisticsQueryHandler(IUnitOfWork _unitOfWork) 
    : IRequestHandler<GetCourseStatisticsQuery, Result<GetCourseStatisticsQueryResponse>>
{
    public async Task<Result<GetCourseStatisticsQueryResponse>> Handle(GetCourseStatisticsQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdWithDetailsAsync(request.Id);
        
        var assignments = await _unitOfWork.AssignmentRepository.GetByCourseIdAsync(request.Id);
        
        if(course is null)
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
        
        var c2rsQuizBanks = course.Quizzes.Count(quiz => quiz.Type == QuizType.C2RS);
        var ipeqQuizBanks = course.Quizzes.Count(quiz => quiz.Type == QuizType.IPEQ);
        var projectsQuizBanks = course.Quizzes.Count(quiz => quiz.Type == QuizType.Projects);
        var mcqQuizBanks = course.Quizzes.Count(quiz => quiz.Type == QuizType.MCQ);
        
        var c2RsAssignments = course.Assignments.Count(a => a.Type == AssignmentType.C2RS);
        var ipeqAssignments = course.Assignments.Count(a => a.Type == AssignmentType.IPEQ);
        var projectsAssignments = course.Assignments.Count(a => a.Type == AssignmentType.Project);
        var mcqAssignments = course.Assignments.Count(a => a.Type == AssignmentType.MCQ);
        
        var statsC2Rs = new AssignmentStatistic(c2rsModules, c2rsQuizBanks,1, c2RsAssignments);
        var statsIpeq = new AssignmentStatistic(ipeqModules,ipeqQuizBanks,1,ipeqAssignments);
        var statsProjects = new AssignmentStatistic(projectsModules, projectsQuizBanks,1,projectsAssignments);
        var statsMcq = new AssignmentStatistic(mcqModules, mcqQuizBanks,1,mcqAssignments);

        var result = new GetCourseStatisticsQueryResponse(
            course.Modules.Count,
            course.Quizzes.Select(q => q.QuizBanks.Count).Sum(),
            1,
            course.Assignments.Count,
            statsC2Rs,
            statsIpeq, 
            statsProjects, 
            statsMcq);
        
        return Result<GetCourseStatisticsQueryResponse>.Success(result);
    }
}
