using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GetGeneratedQuestionsQuery(Guid GeneratedAssigmentId, int Page = 1, int PageSize = 10)
    : IRequest<PagedEntities<GetGeneratedQuestionsQueryResult>>;

public sealed record GetGeneratedQuestionsQueryResult(
    string QuestionText,
    string OptionA,
    string OptionB,
    string OptionC,
    string OptionD,
    string ExplanationCorrect,
    string ExplanationIncorrect);

public sealed class GetGeneratedQuestionsHandler(
    IGeneratedQuestionRepository generatedQuestionRepository,
    ICurrentUserService userService) : IRequestHandler<GetGeneratedQuestionsQuery, PagedEntities<GetGeneratedQuestionsQueryResult>>
{
    public async Task<PagedEntities<GetGeneratedQuestionsQueryResult>> Handle(GetGeneratedQuestionsQuery request, CancellationToken cancellationToken)
    {
        var query = generatedQuestionRepository
            .AsQueryable()
            .Where(x =>
                x.GeneratedAssignmentId == request.GeneratedAssigmentId
                && x.CreateUserId == userService.UserId)
            .OrderByDescending(x => x.CreateDate);
        
        var count = await query.CountAsync(cancellationToken: cancellationToken);
        
        var result = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetGeneratedQuestionsQueryResult(
                x.QuestionText,
                x.OptionA,
                x.OptionB,
                x.OptionC,
                x.OptionD,
                x.ExplanationCorrect,
                x.ExplanationIncorrect))
            .ToList();

        return new PagedEntities<GetGeneratedQuestionsQueryResult>(result, count);
    }
}