namespace KIU.LMS.Application.Features.Questions.Commands;

public sealed record UpdateQuestionBankCommand(Guid Id, string Name) : IRequest<Result>;

public sealed class UpdateQuestionBankCommandValidator : AbstractValidator<UpdateQuestionBankCommand>
{
    public UpdateQuestionBankCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();

        RuleFor(x => x.Id)
            .NotNull();
    }
}


public sealed class UpdateQuestionBankCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<UpdateQuestionBankCommand, Result>
{
    public async Task<Result> Handle(UpdateQuestionBankCommand request, CancellationToken cancellationToken)
    {
        var bank = await _unitOfWork.QuestionBankRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (bank is null)
            return Result.Failure("Can't find question bank");

        bank.Update(request.Name, _current.UserId);

        _unitOfWork.QuestionBankRepository.Update(bank);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Question bank updated successfully");
    }
}
