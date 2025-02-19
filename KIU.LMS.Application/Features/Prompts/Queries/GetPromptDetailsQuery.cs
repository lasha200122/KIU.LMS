namespace KIU.LMS.Application.Features.Prompts.Queries;

public sealed record GetPromptDetailsQuery(Guid Id): IRequest<Result<GetPromptDetailsQueryResponse>>;

public sealed record GetPromptDetailsQueryResponse(Guid Id, string Title, string Value);


public sealed class GetPromptDetailsQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetPromptDetailsQuery, Result<GetPromptDetailsQueryResponse>>
{
    public async Task<Result<GetPromptDetailsQueryResponse>> Handle(GetPromptDetailsQuery request, CancellationToken cancellationToken)
    {
        var prompt = await _unitOfWork.PromptRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (prompt is null)
            return Result<GetPromptDetailsQueryResponse>.Failure("can't find prompt");

        var result = new GetPromptDetailsQueryResponse(
            prompt.Id,
            prompt.Title,
            prompt.Value);

        return Result<GetPromptDetailsQueryResponse>.Success(result);
    }
}