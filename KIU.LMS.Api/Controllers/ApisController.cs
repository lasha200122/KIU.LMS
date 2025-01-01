namespace KIU.LMS.Api.Controllers;

[ApiController]
public abstract class ApisController(ISender _sender) : ControllerBase
{
    protected async Task<IResult> Handle<TRequest, TResponse>(TRequest request)
        where TRequest : IRequest<Result<TResponse>>
    {
        var result = await _sender.Send(request);
        return result.ToResult();
    }
}
