using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Questions.Queries;

public sealed record GetQuestionBankDetailsQuery(Guid Id) : IRequest<Result<GetQuestionBankDetailsQueryResponse>>;

public sealed record GetQuestionBankDetailsQueryResponse(Guid Id, string Name, int Questions, string CreateDate);

public sealed class GetQuestionBankDetailsQueryValidator : AbstractValidator<GetQuestionBankDetailsQuery> 
{
    public GetQuestionBankDetailsQueryValidator() 
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}


public sealed class GetQuestionBankDetailsQueryHandler(IUnitOfWork _unitOfWork, IMongoRepository<Question> _mongoDb) : IRequestHandler<GetQuestionBankDetailsQuery, Result<GetQuestionBankDetailsQueryResponse>>
{
    public async Task<Result<GetQuestionBankDetailsQueryResponse>> Handle(GetQuestionBankDetailsQuery request, CancellationToken cancellationToken)
    {
        var bank = await _unitOfWork.QuestionBankRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (bank is null)
            return Result<GetQuestionBankDetailsQueryResponse>.Failure("Can't find bank");

        var count = await _mongoDb.CountAsync(x => x.QuestionBankId == request.Id.ToString());

        var result = new GetQuestionBankDetailsQueryResponse(
            bank.Id,
            bank.Name,
            (int) count,
            bank.CreateDate.ToString("MMMM/yyyy"));

        return Result<GetQuestionBankDetailsQueryResponse>.Success(result);
    }
}
