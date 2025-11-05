using KIU.LMS.Application.Features.Questions.Commands;
using KIU.LMS.Application.Features.Questions.Commands.AddQuestions;
using KIU.LMS.Application.Features.Questions.Queries;
using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Api.Controllers;

[Route("api/questions")]
[Authorize]
public class QuestionController(ISender sender) : ApiController(sender)
{

    [HttpPost("excel")]
    public async Task<IResult> AddQuestions([FromForm] AddQuestionsCommand request)
    {
        return await Handle(request);
    }

    [HttpGet]
    public async Task<IResult> GetQuestionBanks([FromQuery] GetQuestionBanksQuery request)
    {
        return await Handle<GetQuestionBanksQuery, PagedEntities<GetQuestionBanksQueryResponse>>(request);
    }

    [HttpGet("grid")]
    public async Task<IResult> GetQuestions([FromQuery] GetQuestionsQuery request) 
    {
        return await Handle<GetQuestionsQuery, PagedEntities<Question>>(request);
    }

    [HttpPost]
    public async Task<IResult> AddQuestionBank([FromBody] AddQuestionBankCommand request)
    {
        return await Handle(request);
    }

    [HttpPost("update")]
    public async Task<IResult> UpdateQuestionBank([FromBody] UpdateQuestionBankCommand request)
    {
        return await Handle(request);
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeleteQuestionBank(Guid id)
    {
        return await Handle(new DeleteQuestionBankCommand(id));
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetBankDetails(Guid id) 
    {
        return await Handle<GetQuestionBankDetailsQuery, GetQuestionBankDetailsQueryResponse>(new GetQuestionBankDetailsQuery(id));
    }

    [HttpGet("list/{id}")]
    public async Task<IResult> GetQuestionBanksList(Guid id) 
    {
        return await Handle<GetQuestionBanksListQuery, List<GetQuestionBanksListQueryResponse>>(new GetQuestionBanksListQuery(id));
    }

    [HttpGet("{id}/explain")]
    public async Task<IResult> ExplainQuestion(string id) 
    {
        return await Handle<GeminiQuestionExplainQuery, string>(new GeminiQuestionExplainQuery(id));
    }

    [HttpPost("generate-questions")]
    public async Task<IResult> GenerateQuestions([FromBody] GenerateQuestionsCommand command)
    {
        return await Handle(command);
    }
    
    [HttpGet("generated-questions")]
    [ProducesResponseType(typeof(PagedEntities<GetGeneratedQuestionsQueryResult>), StatusCodes.Status200OK)]
    public async Task<IResult> GetGeneratedQuestions(
        [FromQuery] GetGeneratedQuestionsQuery request) 
    {
        var result = await sender.Send(request);
        return Results.Ok(result);
    }
}
