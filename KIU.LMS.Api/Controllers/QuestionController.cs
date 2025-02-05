using KIU.LMS.Application.Features.Questions.Commands.AddQuestions;

namespace KIU.LMS.Api.Controllers
{
    [Route("api/questions")]
    [Authorize]
    public class QuestionController(ISender sender) : ApiController(sender)
    {
        #region Questions 

        [HttpPost]
        public async Task<IResult> AddQuestions([FromForm] AddQuestionsCommand request)
        {
            return await Handle(request);
        } 

        #endregion Questions
    }
}
