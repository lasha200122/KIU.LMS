using KIU.LMS.Application.Features.LeaderBoard;

namespace KIU.LMS.Api.Controllers;

[Route("api/board")]
public class LeaderBoardController(ISender _sender) : ApiController(_sender)
{
    [HttpGet("leaderboard")]
    public async Task<IResult> GetLeaderBoard([FromQuery] LeaderBoardQuery request)
    {
        return await Handle<LeaderBoardQuery, FinalResponse>(request);
    }

    [HttpGet("leaderboard/filter")]
    public async Task<IResult> GetLeaderBoardFilters() 
    {
        return await Handle<GetLeaderBoardCoursesQuery, List<GetLeaderBoardCoursesQueryResponse>>(new GetLeaderBoardCoursesQuery());
    }
}

