using KIU.LMS.Domain.Common.Enums.Question;

namespace KIU.LMS.Application.Features.QuizSessions.Queries;

public sealed record GetSessionQuestionQuery(string Id) : IRequest<Result<GetSessionQuestionQueryResponse>>;

public sealed record GetSessionQuestionQueryResponse(
    string Id,
    QuestionType Type,
    int? TimeLimit,
    int CurrentIndex,
    DateTimeOffset? StartedAt,
    int Total,
    bool HasExplanation,
    
    // MCQ fields
    string? Text,
    List<AnswerDto>? Options,
    string? ExplanationCorrectAnswer,
    string? ExplanationIncorrectAnswer,
    
    // IPEQ/C2RS fields
    string? TaskDescription,
    string? ReferenceSolution,
    string? CodeGenerationPrompt
);

public sealed record AnswerDto(string Id, string Text);

public sealed class GetSessionQuestionQueryHandler(IExamService _service, IUnitOfWork _unitOfWork) 
    : IRequestHandler<GetSessionQuestionQuery, Result<GetSessionQuestionQueryResponse>>
{
    public async Task<Result<GetSessionQuestionQueryResponse>> Handle(GetSessionQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _service.GetCurrentQuestionAsync(request.Id);

        if (question is null)
            return Result<GetSessionQuestionQueryResponse>.Failure("There are no more questions");

        var session = await _service.GetSessionByIdAsync(request.Id);
        if (session is null)
            return Result<GetSessionQuestionQueryResponse>.Failure("Session not found");

        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == new Guid(session.QuizId));
        if (quiz is null)
            return Result<GetSessionQuestionQueryResponse>.Failure("Quiz not found");

        var result = question.Type switch
        {
            QuestionType.Multiple or QuestionType.Single => new GetSessionQuestionQueryResponse(
                Id: question.QuestionId,
                Type: question.Type,
                TimeLimit: question.TimeLimit,
                CurrentIndex: session.CurrentQuestionIndex + 1,
                StartedAt: question.StartedAt,
                Total: session.Questions.Count,
                HasExplanation: quiz.Explanation,
                
                // MCQ fields
                Text: question.Text,
                Options: question.Options?.Select(x => new AnswerDto(x.Id, x.Text)).ToList(),
                ExplanationCorrectAnswer: question.ExplanationCorrectAnswer,
                ExplanationIncorrectAnswer: question.ExplanationIncorrectAnswer,
                
                // IPEQ/C2RS fields null
                TaskDescription: null,
                ReferenceSolution: null,
                CodeGenerationPrompt: null
            ),
            
            QuestionType.IPEQ or QuestionType.C2RS => new GetSessionQuestionQueryResponse(
                Id: question.QuestionId,
                Type: question.Type,
                TimeLimit: question.TimeLimit,
                CurrentIndex: session.CurrentQuestionIndex + 1,
                StartedAt: question.StartedAt,
                Total: session.Questions.Count,
                HasExplanation: quiz.Explanation,
                
                // MCQ fields null
                Text: null,
                Options: null,
                ExplanationCorrectAnswer: null,
                ExplanationIncorrectAnswer: null,
                
                // IPEQ/C2RS fields
                TaskDescription: question.TaskDescription,
                ReferenceSolution: question.Type == QuestionType.C2RS ? question.ReferenceSolution : null, 
                CodeGenerationPrompt: question.Type ==  QuestionType.IPEQ ? question.CodeGenerationPrompt : null
            ),
            
            _ => throw new NotSupportedException($"Question type {question.Type} is not supported")
        };

        return Result<GetSessionQuestionQueryResponse>.Success(result);
    }
}
