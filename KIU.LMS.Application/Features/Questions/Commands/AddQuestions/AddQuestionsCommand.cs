using KIU.LMS.Domain.Common.Enums.Question;
using KIU.LMS.Domain.Entities.NoSQL;
using Microsoft.AspNetCore.Http;

public sealed record AddQuestionsCommand(IFormFile File, Guid Id, QuizType QuizType) : IRequest<Result>;

public class AddQuestionsCommandValidator : AbstractValidator<AddQuestionsCommand>
{
    public AddQuestionsCommandValidator()
    {
        RuleFor(x => x.File)
            .NotEmpty()
            .WithMessage("Excel file is required");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Question bank ID is required");

        RuleFor(x => x.QuizType)
            .IsInEnum()
            .NotEqual(QuizType.None)
            .WithMessage("Valid quiz type is required");
    }
}

public class AddQuestionsCommandHandler : IRequestHandler<AddQuestionsCommand, Result>
{
    private readonly IMongoRepository<Question> _questionRepository;
    private readonly IExcelProcessor _excelProcessor;

    public AddQuestionsCommandHandler(
        IMongoRepository<Question> questionRepository,
        IExcelProcessor excelProcessor)
    {
        _questionRepository = questionRepository;
        _excelProcessor = excelProcessor;
    }

    public async Task<Result> Handle(AddQuestionsCommand request, CancellationToken cancellationToken)
    {
        List<Question> questions = request.QuizType switch
        {
            QuizType.MCQ => ProcessMcqQuestions(request),
            QuizType.IPEQ or QuizType.C2RS => ProcessIpeqC2rsQuestions(request),
            _ => throw new InvalidOperationException($"Unsupported quiz type: {request.QuizType}")
        };

        if (questions.Count == 0)
            return Result.Failure("No valid questions found in the file");

        await _questionRepository.InsertManyAsync(questions);
        return Result.Success($"{questions.Count} questions added successfully");
    }

    private List<Question> ProcessMcqQuestions(AddQuestionsCommand request)
    {
        var result = _excelProcessor.ProcessQuestionsExcelFile(request.File);

        if (!result.IsValid)
            throw new InvalidOperationException("Excel file contains errors");

        return result.ValidQuestions.Select(q =>
        {
            var options = new List<Option> { new(q.CorrectAnswer, true) };
            options.AddRange(q.IncorrectAnswers.Select(x => new Option(x, false)));

            return new Question(
                request.Id,
                q.Question,
                options,
                q.ExplanationCorrectAnswer,
                q.ExplanationIncorrectAnswer
            );
        }).ToList();
    }

    private List<Question> ProcessIpeqC2rsQuestions(AddQuestionsCommand request)
    {
        var result = _excelProcessor.ProcessIpeqAndC2rsExcelFile(request.File);

        if (!result.IsValid)
            throw new InvalidOperationException("Excel file contains errors");

        var questionType = request.QuizType == QuizType.IPEQ 
            ? QuestionType.IPEQ 
            : QuestionType.C2RS;

        return result.ValidTasks.Select(q =>
            new Question(
                request.Id,
                questionType,
                q.TaskDescription,
                q.CodeSolution,
                q.CodeGenerationPrompt,
                q.CodeGradingPrompt
            )
        ).ToList();
    }
}
