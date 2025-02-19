using KIU.LMS.Application.Features.Questions.Queries;
using KIU.LMS.Application.Features.QuizSessions.Commands;
using KIU.LMS.Application.Features.QuizSessions.Queries;

namespace KIU.LMS.Api.Controllers;

[Route("api/quiz/sessions")]
[Authorize]
public class QuizSessionController(ISender sender) : ApiController(sender)
{
    [HttpPost("{id}/start")]
    public async Task<IResult> StartSession(Guid id) 
    {
        return await Handle<StartSessionCommand, string>(new StartSessionCommand(id));
    }

    [HttpGet("{id}/question")]
    public async Task<IResult> GetSessionQuestion(string id) 
    {
        return await Handle<GetSessionQuestionQuery, GetSessionQuestionQueryResponse>(new GetSessionQuestionQuery(id));
    }

    [HttpPost("submit")]
    public async Task<IResult> SubmitAnswer([FromBody] SubmitAnswerCommand request) 
    {
        return await Handle(request);
    }

    [HttpPost("{id}/finish")]
    public async Task<IResult> FinishSession(string id)
    {
        return await Handle(new FinishExamCommand(id));
    }

    [HttpGet("question/answer/{id}")]
    public async Task<IResult> GetQuestionAnswer(string id) 
    {
        return await Handle<GetQuestionAnswerQuery, List<string>>(new GetQuestionAnswerQuery(id));
    }
}
