namespace KIU.LMS.Application.Features.Sample;

public sealed record SampleQuery(): IRequest<Result<string>>;

public class SampleQueryHandler : IRequestHandler<SampleQuery, Result<string>>
{
    public async Task<Result<string>> Handle(SampleQuery request, CancellationToken cancellationToken)
    {
        await Task.Delay(1);

        return Result<string>.Success("Rame");
    }
}