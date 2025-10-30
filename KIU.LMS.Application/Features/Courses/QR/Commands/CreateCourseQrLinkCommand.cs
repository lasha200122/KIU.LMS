namespace KIU.LMS.Application.Features.Courses.QR.Commands;

public sealed record CreateQrLinkCommand(Guid CourseId) 
    : IRequest<Result<QrLinkResponse>>;

public sealed record QrLinkResponse(string QrUrl, DateTimeOffset ExpiresAt);

public sealed class CreateQrLinkCommandValidator : AbstractValidator<CreateQrLinkCommand>
{
    public CreateQrLinkCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("CourseId is required");
    }
}

public sealed class CreateQrLinkCommandHandler(IQrCodeService qrService)
    : IRequestHandler<CreateQrLinkCommand, Result<QrLinkResponse>>
{
    public async Task<Result<QrLinkResponse>> Handle(CreateQrLinkCommand request, CancellationToken cancellationToken)
    {
        var (qrUrl, expiresAt) = await qrService.CreateQrAsync(request.CourseId);
        return new QrLinkResponse(qrUrl, expiresAt);
    }
}
