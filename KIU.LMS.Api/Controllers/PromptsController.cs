using KIU.LMS.Application.Features.Prompts.Commands;
using KIU.LMS.Application.Features.Prompts.Queries;

namespace KIU.LMS.Api.Controllers;

[Route("api/prompts")]
[Authorize]
public class PromptsController(ISender _sender) : ApiController(_sender)
{
    [HttpPost]
    public async Task<IResult> CreatePrompt([FromBody] CreatePromptCommand request) 
    {
        return await Handle(request);
    }

    [HttpGet]
    public async Task<IResult> GetPromptsList() 
    {
        return await Handle<GetPromptListQuery, IEnumerable<GetPromptListQueryResponse>>(new GetPromptListQuery());
    }
}
