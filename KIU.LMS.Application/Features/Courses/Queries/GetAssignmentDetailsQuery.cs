namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetAssignmentDetailsQuery(Guid Id) : IRequest<Result<GetAssignmentDetailsQueryResponse>>;

public sealed record GetAssignmentDetailsQueryResponse(
    Guid Id,
    Guid CourseId,
    Guid TopicId,
    AssignmentType Type,
    string Name,
    int Order,
    DateTimeOffset? StartDateTime,
    DateTimeOffset? EndDateTime,
    decimal? Score,
    string? Problem,
    string? Code,
    bool IsPublic,
    bool AiGrader,
    SolutionType SolutionType,
    Guid? PromptId,
    bool FullScreen,
    int? RuntimeAttempt,
    bool IsTraining,
    string? PromptText,
    string? CodeSolution);

public class GetAssignmentDetailsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetAssignmentDetailsQuery, Result<GetAssignmentDetailsQueryResponse>>
{
    public async Task<Result<GetAssignmentDetailsQueryResponse>> Handle(GetAssignmentDetailsQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.AssignmentRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (course is null)
            return Result<GetAssignmentDetailsQueryResponse>.Failure("Can't find assigmnemt");

        var response = new GetAssignmentDetailsQueryResponse(
            course.Id,
            course.CourseId,
            course.TopicId,
            course.Type,
            course.Name,
            course.Order,
            course.StartDateTime,
            course.EndDateTime,
            course.Score,
            course.Problem,
            course.Code,
            course.IsPublic,
            course.AIGrader,
            course.SolutionType,
            course.PromptId,
            course.FullScreen,
            course.RuntimeAttempt,
            course.IsTraining,
            course.PromptText,
            course.CodeSolution);

        return Result<GetAssignmentDetailsQueryResponse>.Success(response);
    }
}