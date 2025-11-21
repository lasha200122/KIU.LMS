using KIU.LMS.Application.Features.Voting.Commands;
using KIU.LMS.Application.Features.Voting.Queries;

namespace KIU.LMS.Api.Controllers;

[Route("api/voting")]
[Authorize]
public class VotingController(ISender _sender) : ApiController(_sender)
{
    [HttpPost]
    public async Task<IResult> CreateSession([FromBody] CreateVotingCommand command)
    {
        return await Handle<CreateVotingCommand, Guid>(command);
    }

    [HttpPost("{sessionId}/options")]
    public async Task<IResult> AddOption(Guid sessionId, IFormFile file)
    {
        var command = new AddVotingOptionCommand(sessionId, file);
        return await Handle<AddVotingOptionCommand, Guid>(command);
    }

    [HttpPost("vote")]
    public async Task<IResult> Vote([FromBody] VoteCommand command)
    {
        return await Handle<VoteCommand, Guid>(command);
    }
    
    [HttpGet]
    public async Task<IResult> GetSessions([FromQuery] GetVotingSessionsQuery query)
    {
        return await Handle<GetVotingSessionsQuery, PagedEntities<GetVotingSessionsQueryResponse>>(query);
    }

    [HttpGet("{Id}")]
    public async Task<IResult> GetSessionDetail([FromRoute] GetVotingSessionDetailQuery query)
    {
        return await Handle<GetVotingSessionDetailQuery, GetVotingSessionDetailQueryResponse>(query);
    }

    [HttpGet("options/{OptionId}/votes")]
    public async Task<IResult> GetOptionVotes([FromRoute] Guid OptionId, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
    {
        var query = new GetVotesForOptionQuery(OptionId, PageNumber, PageSize);
        
        return await Handle<GetVotesForOptionQuery, PagedEntities<GetVotesForOptionQueryResponse>>(query);
    }
}
