namespace KIU.LMS.Application.Features.Modules.Commands;

public sealed record UpdateSubModuleCommand(
        Guid Id,
        string Name,
        string? Problem,
        string? Code,
        Guid? PromptId) : IRequest<Result>;

public sealed class UpdateSubModuleCommandValidator : AbstractValidator<UpdateSubModuleCommand>
{
    public UpdateSubModuleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull();
    }
}

public sealed class UpdateSubModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<UpdateSubModuleCommand, Result>
{
    public async Task<Result> Handle(UpdateSubModuleCommand request, CancellationToken cancellationToken)
    {
        var sub = await _unitOfWork.SubModuleRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (sub is null)
            return Result.Failure("Can't find sub module");

        sub.USub(
            request.Name,
            request.Problem,
            request.Code,
            request.PromptId,
            _current.UserId);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Sub module updated successfully");
    }
}
