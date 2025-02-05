namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GetQuestionBanksQuery(string? Name, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<GetQuestionBanksQueryResponse>>>;

public sealed record GetQuestionBanksQueryResponse(Guid Id, string Name);

public class GetQuestionBanksQueryValidator : AbstractValidator<GetQuestionBanksQuery>
{
    public GetQuestionBanksQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetQuestionBanksQueryQueryHandler(IUnitOfWork _unitOfWork) : IRequestHandler<GetQuestionBanksQuery, Result<PagedEntities<GetQuestionBanksQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetQuestionBanksQueryResponse>>> Handle(GetQuestionBanksQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.QuestionBankRepository.GetPaginatedWhereMappedAsync(
            x => string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name),
            request.PageNumber,
            request.PageSize,
            x => new GetQuestionBanksQueryResponse(x.Id, x.Name),
            x => x.Name,
            cancellationToken);

        return Result<PagedEntities<GetQuestionBanksQueryResponse>>.Success(courses)!;
    }
}