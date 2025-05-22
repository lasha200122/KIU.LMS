namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCourseAssignmentsQuery(Guid CourseId, AssignmentType Type, bool IsTraining) : IRequest<Result<IEnumerable<GetCourseAssignmentsQueryResponse>>>;

public sealed record GetCourseAssignmentsQueryResponse(
    Guid Id,
    string Name,
    string Score,
    string TotalScore,
    string StartDate,
    string EndDate,
    string Course,
    int Order);

public sealed class GetCourseAssignmentsHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<GetCourseAssignmentsQuery, Result<IEnumerable<GetCourseAssignmentsQueryResponse>>>
{
    public async Task<Result<IEnumerable<GetCourseAssignmentsQueryResponse>>> Handle(GetCourseAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.AssignmentRepository.GetMappedAsync(
            x => x.CourseId == request.CourseId && x.Type == request.Type && x.StartDateTime.HasValue && x.StartDateTime <= DateTimeOffset.UtcNow && x.IsTraining == request.IsTraining,
            x => new GetCourseAssignmentsQueryResponse(
                x.Id,
                x.Name,
                x.Solutions.Any(x => x.UserId == _current.UserId) ? x.Solutions.Where(x => x.UserId == _current.UserId).OrderByDescending(x => x.CreateDate).First().Grade : string.Empty,
                x.Score.HasValue? x.Score.Value.ToString() : "-",
                x.StartDateTime.HasValue? x.StartDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm"): string.Empty,
                x.EndDateTime.HasValue ? x.EndDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty,
                x.Topic.Name,
                x.Order),
            cancellationToken);

        return Result<IEnumerable<GetCourseAssignmentsQueryResponse>>.Success(result);
    }
}