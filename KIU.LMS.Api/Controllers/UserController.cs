namespace KIU.LMS.Api.Controllers;

[Route("api/users")]
public class UserController(ISender _sender) : ApiController(_sender)
{
    [HttpGet]
    [Authorize]
    public async Task<IResult> GetStudents([FromQuery] GetStudentsListQuery request)
    {
        return await Handle<GetStudentsListQuery, PagedEntities<GetStudentsListQueryResponse>>(request);
    }

    [HttpPost]
    public async Task<IResult> RegisterUser([FromBody] RegisterUserCommand request)
    {
        return await Handle(request);
    }
    
    [HttpPost("excel")]
    public async Task<IResult> RegisterUsers([FromForm] RegisterUsersCommand request)
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
    [Authorize]
    public async Task<IResult> ResetPasswordRequest(Guid id)
    {
        return await Handle(new ResetPasswordRequestCommand(id));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IResult> DeactivateUser(Guid id) 
    {
        return await Handle(new DeactivateUserCommand(id));
    }

    [HttpPut("reset-password")]
    [Authorize]
    public async Task<IResult> ResetPassword([FromBody] ResetPasswordCommand request)
    {
        return await Handle(request);
    }

    [HttpPost("reset-password-by-email")]
    public async Task<IResult> ResetPasswordByEmail([FromQuery] ResetPasswordRequestByEmailCommand command)
    {
        return await Handle(command);
    }

    [HttpPost("register/qr")]
    [AllowAnonymous]
    public async Task<IResult> RegisterUserByQR([FromBody] RegisterUserByQRCommand request) 
    {
        return await Handle(request);
    }
}
