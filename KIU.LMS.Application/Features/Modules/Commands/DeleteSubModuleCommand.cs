namespace KIU.LMS.Application.Features.Modules.Commands;


public sealed record DeleteSubModuleCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteSubModuleCommandValidator : AbstractValidator<DeleteSubModuleCommand>
{
    public DeleteSubModuleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}

public sealed class DeleteSubModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<DeleteSubModuleCommand, Result>
{
    public async Task<Result> Handle(DeleteSubModuleCommand request, CancellationToken cancellationToken)
    {
        var sub = await _unitOfWork.SubModuleRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (sub is null)
            return Result.Failure("Can't find sub module");

        sub.Delete(_current.UserId, DateTimeOffset.UtcNow);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Sub module deleted successfully");
    }
}
