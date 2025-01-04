namespace KIU.LMS.Api.Controllers;

[Route("api/auth")]
public class AuthController(ISender _sender) : ApiController(_sender)
{
    [HttpPost]
    public async Task<IResult> Login([FromBody] LoginCommand request) 
    {
        return await Handle<LoginCommand, LoginCommandResponse>(request);
    }

    [HttpPost("refresh-token")]
    public async Task<IResult> RefreshToken([FromBody] RefreshTokenCommand request) 
    {
        return await Handle<RefreshTokenCommand, LoginCommandResponse>(request);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IResult> Logout() 
    {
        return await Handle(new LogoutCommand());
    }
}
