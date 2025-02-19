using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GetQuestionAnswerQuery(string Id) : IRequest<Result<List<string>>>;

public sealed class GetQuestionAnswerQueryHandler(IMongoRepository<Question> _mongoDb) : IRequestHandler<GetQuestionAnswerQuery, Result<List<string>>>
{
    public async Task<Result<List<string>>> Handle(GetQuestionAnswerQuery request, CancellationToken cancellationToken)
    {
        var question = await _mongoDb.GetByIdAsync(request.Id);

        if (question == null)
            return Result<List<string>>.Failure("Can't find question");

        var answer = question.Options.Where(x => x.IsCorrect).Select(x => x.Id).ToList();

        return Result<List<string>>.Success(answer);
    }
}