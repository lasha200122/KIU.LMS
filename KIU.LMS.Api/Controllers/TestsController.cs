using KIU.LMS.Application.Features.Sample;

namespace KIU.LMS.Api.Controllers;

[Route("api/[controller]")]
public class TestsController(ISender _sender) : ApisController(_sender)
{
    [HttpGet]
    public async Task<IResult> Test() 
    {
        return await Handle<SampleQuery, string>(new SampleQuery());
    }
}
