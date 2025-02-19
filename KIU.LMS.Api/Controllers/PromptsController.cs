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

    [HttpPost("update")]
    public async Task<IResult> UpdatePrompt([FromBody] UpdatePromptCommand request)
    {
        return await Handle(request);
    }

    [HttpGet]
    public async Task<IResult> GetPromptsList() 
    {
        return await Handle<GetPromptListQuery, IEnumerable<GetPromptListQueryResponse>>(new GetPromptListQuery());
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetPromptDetails(Guid id) 
    {
        return await Handle<GetPromptDetailsQuery, GetPromptDetailsQueryResponse>(new GetPromptDetailsQuery(id));
    }

    [HttpDelete("{id}")]
    public async Task<IResult> DeletePrompt(Guid id)
    {
        return await Handle(new DeletePromptCommand(id));
    }
}
