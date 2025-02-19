namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GetTopicDetailsQuery(Guid Id) : IRequest<Result<GetTopicDetailsQueryResponse>>;

public sealed record GetTopicDetailsQueryResponse(Guid Id, string Name, DateTimeOffset StartTime, DateTimeOffset EndTime);

public sealed class GetTopicDetailsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetTopicDetailsQuery, Result<GetTopicDetailsQueryResponse>>
{
    public async Task<Result<GetTopicDetailsQueryResponse>> Handle(GetTopicDetailsQuery request, CancellationToken cancellationToken)
    {
        var topic = await _unitOfWork.TopicRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (topic is null)
            return Result<GetTopicDetailsQueryResponse>.Failure("Can't find topic");

        var result = new GetTopicDetailsQueryResponse(
            topic.Id,
            topic.Name,
            topic.StartDateTime.ToLocalTime(),
            topic.EndDateTime.ToLocalTime());

        return Result<GetTopicDetailsQueryResponse>.Success(result);
    }
}