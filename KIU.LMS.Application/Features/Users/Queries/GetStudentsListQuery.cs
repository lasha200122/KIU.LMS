namespace KIU.LMS.Application.Features.Users.Queries;

public sealed record GetStudentsListQuery(string? Email, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<GetStudentsListQueryResponse>>>;

public sealed record GetStudentsListQueryResponse(Guid Id, string FullName, string Email);

public class GetStudentsListQueryValidator : AbstractValidator<GetStudentsListQuery> 
{
    public GetStudentsListQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetStudentsListQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetStudentsListQuery, Result<PagedEntities<GetStudentsListQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetStudentsListQueryResponse>>> Handle(GetStudentsListQuery request, CancellationToken cancellationToken)
    {
        var students = await _unitOfWork.UserRepository.GetPaginatedWhereMappedAsync(
            x => x.Role == UserRole.Student && (string.IsNullOrEmpty(request.Email) || x.Email.Contains(request.Email)),
            request.PageNumber,
            request.PageSize,
            x => new GetStudentsListQueryResponse(x.Id, $"{x.FirstName} {x.LastName}", x.Email),
            x => x.Email,
            cancellationToken);

        return Result<PagedEntities<GetStudentsListQueryResponse>>.Success(students)!;
    }
}