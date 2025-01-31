namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetTopicsListQuery(Guid Id) : IRequest<Result<IEnumerable<GetTopicsListQueryResponse>>>;

public sealed record GetTopicsListQueryResponse(Guid Id, string Name);


public sealed class GetTopicsListQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetTopicsListQuery, Result<IEnumerable<GetTopicsListQueryResponse>>>
{
    public async Task<Result<IEnumerable<GetTopicsListQueryResponse>>> Handle(GetTopicsListQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TopicRepository.GetMappedAsync(
            x => x.CourseId == request.Id,
            x => new GetTopicsListQueryResponse(x.Id, x.Name),
            cancellationToken);

        return Result<IEnumerable<GetTopicsListQueryResponse>>.Success(result);
    }
}
