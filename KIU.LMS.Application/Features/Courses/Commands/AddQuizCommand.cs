namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record AddQuizCommand(
    Guid CourseId,
    Guid TopicId,
    string Title,
    QuizType Type,
    int Order,
    int? Attempts,
    DateTimeOffset StartDateTime,
    DateTimeOffset? EndDateTime,
    decimal? Score,
    bool Explanation,
    int? TimePerQuestion,
    decimal? MinusScore,
    DateTimeOffset? PublicTill,
    List<QuestionBankItem> Banks) : IRequest<Result>;

public sealed record QuestionBankItem(Guid Id, int Amount);

public sealed class AddQuizCommandValidator : AbstractValidator<AddQuizCommand> 
{
    public AddQuizCommandValidator() 
    {
        RuleFor(x => x.CourseId)
            .NotNull();

        RuleFor(x => x.TopicId)
            .NotNull();

        RuleFor(x => x.Title)
            .NotNull();

        RuleFor(x => x.Type)
            .NotNull();

        RuleFor(x => x.Order)
            .NotNull();

        RuleFor(x => x.StartDateTime)
            .NotNull();

        RuleFor(x => x.Banks)
            .NotNull();

        RuleFor(x => x.Banks)
            .NotNull()
            .NotEmpty().WithMessage("მინიმუმ ერთი Question Bank მაინც უნდა იყოს მითითებული")
            .ForEach(bank => {
                bank.ChildRules(item => {
                    item.RuleFor(x => x.Amount)
                        .GreaterThan(0)
                        .WithMessage("კითხვების რაოდენობა უნდა იყოს 0-ზე მეტი");
                });
            });
    }
}

public sealed class AddQuizCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<AddQuizCommand, Result>
{
    public async Task<Result> Handle(AddQuizCommand request, CancellationToken cancellationToken)
    {
        if (!request.Banks.Any())
            return Result.Failure("Question Banks is required");

        var courseExist = await _unitOfWork.CourseRepository.ExistsAsync(x => x.Id == request.CourseId);

        if (!courseExist)
            return Result.Failure("Can't find course");

        var topicExist = await _unitOfWork.TopicRepository.ExistsAsync(x => x.Id == request.TopicId);

        if (!topicExist)
            return Result.Failure("Can't find topic");

        var quiz = new Quiz(
            Guid.NewGuid(),
            request.CourseId,
            request.TopicId,
            request.Title,
            request.Type,
            request.Order,
            request.Attempts,
            request.StartDateTime,
            request.EndDateTime,
            request.Score,
            request.Explanation,
            request.TimePerQuestion,
            request.MinusScore,
            request.PublicTill,
            _current.UserId);

        var quizBanks = request.Banks.Select(x => new QuizBank(
            Guid.NewGuid(),
            quiz.Id,
            x.Id,
            x.Amount,
            _current.UserId))
        .ToList();

        await _unitOfWork.QuizRepository.AddAsync(quiz);
        await _unitOfWork.QuizBankRepository.AddRangeAsync(quizBanks);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Quiz Created Successfully");
    }
}
