namespace KIU.LMS.Api.Controllers;

[Route("api/auth")]
public class AuthController(ISender _sender) : ApiController(_sender)
{
    [HttpPost]
    public async Task<IResult> Login([FromBody] LoginCommand request) 
    {
        return await Handle<LoginCommand, LoginCommandResponse>(request);
    }
}
