using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GetQuestionsQuery(Guid Id, int PageNumber, int PageSize) : IRequest<Result<PagedEntities<Question>>>;


public class GetQuestionsQueryValidator : AbstractValidator<GetQuestionsQuery>
{
    public GetQuestionsQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
        RuleFor(x => x.Id)
            .NotNull();
    }
}


public sealed class GetQuestionsQueryHandler(IMongoRepository<Question> _mongoDb) : IRequestHandler<GetQuestionsQuery, Result<PagedEntities<Question>>>
{
    public async Task<Result<PagedEntities<Question>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        return await _mongoDb.GetPagedDataAsync(x => x.QuestionBankId == request.Id.ToString(), request.PageNumber, request.PageSize);
    }
}