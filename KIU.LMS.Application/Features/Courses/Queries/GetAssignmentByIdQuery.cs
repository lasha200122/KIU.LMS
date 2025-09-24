namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetAssignmentByIdQuery(Guid Id) : IRequest<Result<GetAssignmentByIdQueryResponse>>;

public sealed record GetAssignmentByIdQueryResponse(
    Guid Id,
    string Name,
    string StartDateTime,
    string EndDateTime,
    string? Score,
    string? Problem,
    string? Code,
    bool IsPublic,
    string Topic,
    string Grade,
    string Feedback,
    bool SubmitAllowed,
    string Value,
    string Submeted,
    bool FullScreen,
    int? RuntimeAttempt,
    SolutionType SolutionType,
    string? PromptText,
    string? CodeSolution,
    Guid StudentId);


public class GetAssignmentByIdQueryHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<GetAssignmentByIdQuery, Result<GetAssignmentByIdQueryResponse>>
{
    public async Task<Result<GetAssignmentByIdQueryResponse>> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.AssignmentRepository.SingleOrDefaultAsync(x => x.Id == request.Id, x => x.Topic);

        if (assignment is null)
            return Result<GetAssignmentByIdQueryResponse>.Failure("Can't find Assignment");

        var studentSolution = await _unitOfWork.SolutionRepository.FirstOrDefaultSortedAsync(
            x => x.AssignmentId == assignment.Id && x.UserId == _current.UserId,
            x => x.CreateDate,
            true);

        var result = new GetAssignmentByIdQueryResponse(
            assignment.Id,
            assignment.Name,
            assignment.StartDateTime.HasValue? assignment.StartDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty,
            assignment.EndDateTime.HasValue ? assignment.EndDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty,
            assignment.Score.HasValue? assignment.Score.ToString() : string.Empty,
            assignment.Problem,
            assignment.Code,
            assignment.IsPublic,
            assignment.Topic.Name,
            studentSolution?.Grade?? string.Empty,
            studentSolution?.FeedBack ?? string.Empty,
            assignment.EndDateTime.HasValue? DateTimeOffset.UtcNow <= assignment.EndDateTime.Value : true,
            studentSolution?.Value?? string.Empty,
            studentSolution == null ? string.Empty :studentSolution.CreateDate.ToLocalTime().ToString("MMM dd, HH:mm"),
            assignment.FullScreen,
            assignment.RuntimeAttempt,
            assignment.SolutionType,
            assignment.PromptText,
            assignment.CodeSolution,
            studentSolution?.UserId?? Guid.Empty);

        return Result<GetAssignmentByIdQueryResponse>.Success(result);
    }
}