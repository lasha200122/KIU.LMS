namespace KIU.LMS.Api.Controllers;

[Route("api/quizzes")]
[Authorize]
public class QuizzesController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<IResult> AddQuiz([FromBody] AddQuizCommand request) 
    {
        return await Handle(request);
    }
}
