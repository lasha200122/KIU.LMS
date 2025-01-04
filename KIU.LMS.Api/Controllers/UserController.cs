namespace KIU.LMS.Api.Controllers;

//[Authorize]
[Route("api/users")]
public class UserController(ISender _sender) : ApiController(_sender)
{
    [HttpPost]
    public async Task<IResult> RegisterUser([FromBody] RegisterUserCommand request)
    {
        return await Handle(request);
    }

    [HttpPatch]
    [AllowAnonymous]
    public async Task<IResult> VerifyEmail([FromBody] VerifyEmailCommand request)
    {
        return await Handle(request);
    }

    [HttpPatch("{id}/reset-password-request")]
    public async Task<IResult> ResetPasswordRequest(Guid id)
    {
        return await Handle(new ResetPasswordRequestCommand(id));
    }

    [HttpPut("reset-password")]
    public async Task<IResult> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        return await Handle(request);
    }
}
