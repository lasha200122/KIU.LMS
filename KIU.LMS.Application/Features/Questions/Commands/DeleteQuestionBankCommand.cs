using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Application.Features.Questions.Commands;

public sealed record DeleteQuestionBankCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteQuestionBankCommandHandler(
    IUnitOfWork _unitOfWork,
    ICurrentUserService _current,
    IMongoRepository<Question> _mongo) : IRequestHandler<DeleteQuestionBankCommand, Result>
{
    public async Task<Result> Handle(DeleteQuestionBankCommand request, CancellationToken cancellationToken)
    {
        var bank = await _unitOfWork.QuestionBankRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (bank is null)
            return Result.Failure("Can't find question bank");

        bank.Delete(_current.UserId, DateTimeOffset.UtcNow);

        _unitOfWork.QuestionBankRepository.Update(bank);

        await _unitOfWork.SaveChangesAsync();

        await _mongo.DeleteManyAsync(x => x.QuestionBankId == $"{request.Id}");

        return Result.Success();
    }
}