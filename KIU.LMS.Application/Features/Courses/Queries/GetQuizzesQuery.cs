namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetQuizzesQuery(Guid Id, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<GetQuizzesQueryResponse>>>;

public sealed record GetQuizzesQueryResponse(
    Guid Id,
    string Name,
    string Type);

public sealed class GetQuizzesQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetQuizzesQuery, Result<PagedEntities<GetQuizzesQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetQuizzesQueryResponse>>> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.QuizRepository.GetPaginatedWhereMappedAsync(
            x => x.CourseId == request.Id,
            request.PageNumber,
            request.PageSize,
            x => new GetQuizzesQueryResponse(
                x.Id,
                x.Title,
                x.Type.ToString()),
            x => x.CreateDate,
            cancellationToken);

        return Result<PagedEntities<GetQuizzesQueryResponse>>.Success(result);
    }
}