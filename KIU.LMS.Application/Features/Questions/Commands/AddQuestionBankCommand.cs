namespace KIU.LMS.Application.Features.Questions.Commands;

public sealed record AddQuestionBankCommand(string Name) : IRequest<Result>;

public sealed class AddQuestionBankCommandValidator : AbstractValidator<AddQuestionBankCommand> 
{
    public AddQuestionBankCommandValidator() 
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}


public sealed class AddQuestionBankCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<AddQuestionBankCommand, Result>
{
    public async Task<Result> Handle(AddQuestionBankCommand request, CancellationToken cancellationToken)
    {
        var questionBank = new QuestionBank(
            Guid.NewGuid(),
            request.Name,
            _current.UserId);

        await _unitOfWork.QuestionBankRepository.AddAsync(questionBank);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Question bank created successfully");
    }
}
