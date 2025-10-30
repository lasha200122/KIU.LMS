namespace KIU.LMS.Application.Features.Courses.QR;

public sealed record GetCourseRegistrationRedirectUrlQuery(string Token) 
    : IRequest<Result>;

public sealed class GetRedirectUrlQueryValidator : AbstractValidator<GetCourseRegistrationRedirectUrlQuery>
{
    public GetRedirectUrlQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token cannot be empty");
    }
}

public sealed class GetRedirectUrlQueryHandler(IQrCodeService qrService)
    : IRequestHandler<GetCourseRegistrationRedirectUrlQuery, Result>
{
    public async Task<Result> Handle(GetCourseRegistrationRedirectUrlQuery request, CancellationToken cancellationToken)
    {
        var redirectUrl = await qrService.GetRedirectUrlAsync(request.Token);
        if (redirectUrl is null)
            return Result.Failure("QR code expired or invalid");

        if (!redirectUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            redirectUrl = redirectUrl.Replace("http://", "https://");

        return Result.Success(redirectUrl);
    }
}
