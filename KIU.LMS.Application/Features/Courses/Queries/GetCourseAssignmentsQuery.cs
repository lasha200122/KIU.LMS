namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCourseAssignmentsQuery(Guid CourseId, AssignmentType Type) : IRequest<Result<IEnumerable<GetCourseAssignmentsQueryResponse>>>;

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
            x => x.CourseId == request.CourseId && x.Type == request.Type,
            x => new GetCourseAssignmentsQueryResponse(
                x.Id,
                x.Name,
                "0",
                x.Score.HasValue? x.Score.Value.ToString() : "0",
                x.StartDateTime.HasValue? x.StartDateTime.Value.ToString("MMM dd, HH:mm"): string.Empty,
                x.EndDateTime.HasValue ? x.EndDateTime.Value.ToString("MMM dd, HH:mm") : string.Empty,
                x.Topic.Name,
                x.Order
                ),
            cancellationToken);

        return Result<IEnumerable<GetCourseAssignmentsQueryResponse>>.Success(result);
    }
}