using KIU.LMS.Application.Features.Courses.QR;
using KIU.LMS.Application.Features.Courses.QR.Commands;

namespace KIU.LMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QrCodeController(ISender sender) : ApiController(sender)
{
    [Authorize]
    [HttpGet("get-qr-link")]
    public async Task<IResult> GetQrLink([FromQuery] Guid courseId)
    {
        return await Handle<CreateQrLinkCommand, QrLinkResponse>(new CreateQrLinkCommand(courseId));
    }

    [HttpGet("{token}")]
    public async Task<IResult> RedirectToCourse(string token)
    {
        var result = await sender.Send(new GetCourseRegistrationRedirectUrlQuery(token));
        return result.IsSuccess 
            ? Results.Redirect(result.Message)
            : Results.StatusCode(404);
    }
}
