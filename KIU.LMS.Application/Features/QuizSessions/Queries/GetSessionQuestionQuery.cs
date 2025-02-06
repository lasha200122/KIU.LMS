namespace KIU.LMS.Application.Features.QuizSessions.Queries;

public sealed record GetSessionQuestionQuery(string Id) : IRequest<Result<GetSessionQuestionQueryResponse>>;

public sealed record GetSessionQuestionQueryResponse(
    string Id,
    string Text,
    int? TimeLimit,
    List<AnswerDto> Options,
    int CurrentIndex,
    DateTimeOffset? StartedAt,
    int Total);

public sealed record AnswerDto(string Id, string Text);

public sealed class GetSessionQuestionQueryHandler(IExamService _service) : IRequestHandler<GetSessionQuestionQuery, Result<GetSessionQuestionQueryResponse>>
{
    public async Task<Result<GetSessionQuestionQueryResponse>> Handle(GetSessionQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _service.GetCurrentQuestionAsync(request.Id);

        if (question is null)
            return Result<GetSessionQuestionQueryResponse>.Failure("The is no more questions");

        var session = await _service.GetSessionByIdAsync(request.Id);


        var result = new GetSessionQuestionQueryResponse(
            question.QuestionId,
            question.Text,
            question.TimeLimit,
            question.Options.Select(x => new AnswerDto(x.Id, x.Text)).ToList(),
            session!.CurrentQuestionIndex + 1,
            question.StartedAt,
            session!.Questions.Count());

        return Result<GetSessionQuestionQueryResponse>.Success(result);
    }
}
