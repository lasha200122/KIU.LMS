namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record class GetCourseStudentsQuery(Guid Id, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<GetCourseStudentsQueryResponse>>>;

public sealed record GetCourseStudentsQueryResponse(Guid Id, string FullName, string Email, string AddDate, string AccessTill);

public class GetCourseStudentsQueryValidator : AbstractValidator<GetCourseStudentsQuery>
{
    public GetCourseStudentsQueryValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetCourseStudentsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetCourseStudentsQuery, Result<PagedEntities<GetCourseStudentsQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetCourseStudentsQueryResponse>>> Handle(GetCourseStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _unitOfWork.UserCourseRepository.GetPaginatedWhereMappedAsync(
            x => x.CourseId == request.Id,
            request.PageNumber,
            request.PageSize,
            x => new GetCourseStudentsQueryResponse(x.Id, $"{x.User.FirstName} {x.User.LastName}", x.User.Email, x.CreateDate.ToString("dd/MM/yyyy"), x.CanAccessTill.ToString("dd/MM/yyyy")),
            x => x.User.FirstName,
            cancellationToken);

        return Result<PagedEntities<GetCourseStudentsQueryResponse>>.Success(students)!;
    }
}
