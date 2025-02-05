namespace KIU.LMS.Application.Features.Prompts.Queries;

public sealed record GetPromptListQuery(): IRequest<Result<IEnumerable<GetPromptListQueryResponse>>>;

public sealed record GetPromptListQueryResponse(Guid Id, string Name);


public sealed class GetPromptListQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetPromptListQuery, Result<IEnumerable<GetPromptListQueryResponse>>>
{
    public async Task<Result<IEnumerable<GetPromptListQueryResponse>>> Handle(GetPromptListQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.PromptRepository.GetMappedAsync(x => true, x => new GetPromptListQueryResponse(
            x.Id,
            x.Title),
            cancellationToken);

        return Result<IEnumerable<GetPromptListQueryResponse>>.Success(result);
    }
}
