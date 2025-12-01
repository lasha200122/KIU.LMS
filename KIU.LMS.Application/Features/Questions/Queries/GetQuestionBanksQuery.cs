namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GetQuestionBanksQuery(Guid ModuleId, string? Name, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<GetQuestionBanksQueryResponse>>>;

public sealed record GetQuestionBanksQueryResponse(Guid Id, string Name, QuizType Type);

public class GetQuestionBanksQueryValidator : AbstractValidator<GetQuestionBanksQuery>
{
    public GetQuestionBanksQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetQuestionBanksQueryQueryHandler(IUnitOfWork _unitOfWork) 
    : IRequestHandler<GetQuestionBanksQuery, Result<PagedEntities<GetQuestionBanksQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetQuestionBanksQueryResponse>>> Handle(
        GetQuestionBanksQuery request, 
        CancellationToken cancellationToken)
    {
        var questionBanks = await _unitOfWork.QuestionBankRepository.GetPaginatedWhereMappedAsync(
            x => x.ModuleId == request.ModuleId && 
                 (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)),
            request.PageNumber,
            request.PageSize,
            x => new GetQuestionBanksQueryResponse(
                x.Id, 
                x.Name,
                x.QuizBanks.FirstOrDefault() != null 
                    ? x.QuizBanks.First().Quiz.Type 
                    : QuizType.None
            ),
            x => x.Name,
            cancellationToken);

        return Result<PagedEntities<GetQuestionBanksQueryResponse>>.Success(questionBanks)!;
    }
}
