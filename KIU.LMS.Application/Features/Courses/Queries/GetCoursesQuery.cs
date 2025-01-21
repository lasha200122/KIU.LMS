namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetCoursesQuery(string? Name, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<GetCoursesQueryResponse>>>;

public sealed record GetCoursesQueryResponse(Guid Id, string Name);

public class GetCoursesQueryValidator : AbstractValidator<GetCoursesQuery>
{
    public GetCoursesQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetCoursesQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCoursesQuery, Result<PagedEntities<GetCoursesQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetCoursesQueryResponse>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.CourseRepository.GetPaginatedWhereMappedAsync(
            x => string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name),
            request.PageNumber,
            request.PageSize,
            x => new GetCoursesQueryResponse(x.Id, x.Name),
            x => x.Name,
            cancellationToken);

        return Result<PagedEntities<GetCoursesQueryResponse>>.Success(courses)!;
    }
}