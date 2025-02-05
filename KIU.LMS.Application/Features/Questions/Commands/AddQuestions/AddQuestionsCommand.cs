using KIU.LMS.Domain.Common.Enums.Question;
using KIU.LMS.Domain.Entities.NoSQL;
using Microsoft.AspNetCore.Http;

namespace KIU.LMS.Application.Features.Questions.Commands.AddQuestions
{
    public sealed record AddQuestionsCommand(IFormFile File, Guid Id) : IRequest<Result>;

    public class AddQuestionsCommandValidator : AbstractValidator<AddQuestionsCommand>
    {
        public AddQuestionsCommandValidator()
        {
            RuleFor(x => x.File)
                .NotEmpty();

            RuleFor(x => x.Id)
                .NotNull();
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
            var result = _excelProcessor.ProcessQuestionsExcelFile(request.File);

            if (!result.IsValid)
                return Result.Failure("Excel file contains errors");

            var questions = result.ValidQuestions.Select(q =>
            {
                var options = new List<Option>
            {
                new Option(q.CorrectAnswer, true)
            };

                options.AddRange(q.IncorrectAnswers.Select(x => new Option(x, false)));

                return new Question(
                    request.Id,
                    q.Question,
                    QuestionType.Single,
                    options
                );
            }).ToList();

            await _questionRepository.InsertManyAsync(questions);
            return Result.Success("Questions added successfully");
        }
    }
}