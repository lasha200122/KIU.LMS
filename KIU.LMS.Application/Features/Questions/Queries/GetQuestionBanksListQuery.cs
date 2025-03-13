using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GetQuestionBanksListQuery(Guid Id) : IRequest<Result<List<GetQuestionBanksListQueryResponse>>>;

public sealed record GetQuestionBanksListQueryResponse(Guid Id, string Title, int Amount);

public sealed class GetQuestionBanksListQueryHandler(IUnitOfWork _unitOfWork, IMongoRepository<Question> _mongoDb) : IRequestHandler<GetQuestionBanksListQuery, Result<List<GetQuestionBanksListQueryResponse>>>
{
    public async Task<Result<List<GetQuestionBanksListQueryResponse>>> Handle(GetQuestionBanksListQuery request, CancellationToken cancellationToken)
    {
        //var questionBankIds = await _unitOfWork.QuestionBankRepository.GetMappedAsync(x => x.Module.CourseId == request.Id, x => x.Id.ToString());

        var questions = await _mongoDb.GetAllAsync();

        var questionBanksWithCounts = questions
            .GroupBy(q => q.QuestionBankId)
            .ToDictionary(
                group => group.Key,
                group => group.Count());

        var ids = questionBanksWithCounts.Keys.ToHashSet();

        var banks = await _unitOfWork.QuestionBankRepository.GetWhereAsync(
            x => ids.Contains(x.Id.ToString()),
            cancellationToken);

        var result = banks.Select(x => new GetQuestionBanksListQueryResponse(
                x.Id,
                x.Name,
                questionBanksWithCounts.GetValueOrDefault(x.Id.ToString())
            )).ToList();

        return Result<List<GetQuestionBanksListQueryResponse>>.Success(result);
    }
}