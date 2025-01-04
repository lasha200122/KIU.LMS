namespace KIU.LMS.Api.Controllers;

[ApiController]
public abstract class ApiController(ISender _sender) : ControllerBase
{
    protected async Task<IResult> Handle<TRequest, TResponse>(TRequest request)
        where TRequest : IRequest<Result<TResponse>>
    {
        var result = await _sender.Send(request);
        return result.ToResult();
    }

    protected async Task<IResult> Handle<TRequest>(TRequest request)
        where TRequest : IRequest<Result>
    {
        var result = await _sender.Send(request);
        return result.ToResult();
    }

    protected async Task<IResult> HandleFile<TRequest>(TRequest request, string contentType, string fileName)
        where TRequest : IRequest<Result<byte[]>>
    {
        var result = await _sender.Send(request);
        return result.ToFileResult(contentType, fileName);
    }
}
