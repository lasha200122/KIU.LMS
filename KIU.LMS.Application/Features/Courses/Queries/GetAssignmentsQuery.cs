namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetAssignmentsQuery(Guid Id, int PageNumber, int PageSize, AssignmentType? Type) : IRequest<Result<PagedEntities<GetAssignmentsQueryesponse>>>;

public sealed record GetAssignmentsQueryesponse(
    Guid Id,
    string Name,
    string Type);


public sealed class GetAssignmentsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetAssignmentsQuery, Result<PagedEntities<GetAssignmentsQueryesponse>>>
{
    public async Task<Result<PagedEntities<GetAssignmentsQueryesponse>>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.AssignmentRepository.GetPaginatedWhereMappedAsync(
            x => x.CourseId == request.Id && (!request.Type.HasValue || x.Type == request.Type.Value),
            request.PageNumber,
            request.PageSize,
            x => new GetAssignmentsQueryesponse(
                x.Id,
                x.Name,
                x.Type.ToString()),
            x => x.CreateDate,
            cancellationToken);

        return Result<PagedEntities<GetAssignmentsQueryesponse>>.Success(result);
    }
}
