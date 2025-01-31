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
    string Topic);


public class GetAssignmentByIdQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetAssignmentByIdQuery, Result<GetAssignmentByIdQueryResponse>>
{
    public async Task<Result<GetAssignmentByIdQueryResponse>> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.AssignmentRepository.SingleOrDefaultAsync(x => x.Id == request.Id, x => x.Topic);

        if (assignment is null)
            return Result<GetAssignmentByIdQueryResponse>.Failure("Can't find Assignment");

        var result = new GetAssignmentByIdQueryResponse(
            assignment.Id,
            assignment.Name,
            assignment.StartDateTime.HasValue? assignment.StartDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty,
            assignment.EndDateTime.HasValue ? assignment.EndDateTime.Value.ToLocalTime().ToString("MMM dd, HH:mm") : string.Empty,
            assignment.Score.HasValue? assignment.Score.ToString() : string.Empty,
            assignment.Problem,
            assignment.Code,
            assignment.IsPublic,
            assignment.Topic.Name);

        return Result<GetAssignmentByIdQueryResponse>.Success(result);
    }
}