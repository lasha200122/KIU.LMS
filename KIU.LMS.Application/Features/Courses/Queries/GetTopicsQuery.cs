
namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetTopicsQuery(Guid Id, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<GetTopicsQueryResponse>>>;

public sealed record GetTopicsQueryResponse(
    Guid Id,
    string Name,
    string Date,
    string Time);


public sealed class GetTopicsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetTopicsQuery, Result<PagedEntities<GetTopicsQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetTopicsQueryResponse>>> Handle(GetTopicsQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TopicRepository.GetPaginatedWhereMappedAsync(
            x => x.CourseId == request.Id,
            request.PageNumber,
            request.PageSize,
            x => new GetTopicsQueryResponse(
                x.Id,
                x.Name,
                x.StartDateTime.HasValue? x.StartDateTime.Value.ToString("dd/MM/yyyy") : string.Empty,
                $"{ x.StartDateTime.Value.ToString("HH:mm")?? string.Empty } - {x.EndDateTime.Value.ToString("HH:mm")?? string.Empty}"),
            x => x.StartDateTime,
            cancellationToken);

        return Result<PagedEntities<GetTopicsQueryResponse>>.Success(result);
    }
}
