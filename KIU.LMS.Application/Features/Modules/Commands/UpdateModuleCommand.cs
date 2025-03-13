namespace KIU.LMS.Application.Features.Modules.Commands;

public sealed record UpdateModuleCommand(Guid Id, string Name) : IRequest<Result>;

public class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand> 
{
    public UpdateModuleCommandValidator() 
    {
        RuleFor(x => x.Id)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull();
    }
}

public sealed class UpdateModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<UpdateModuleCommand, Result>
{
    public async Task<Result> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await _unitOfWork.ModuleRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (module is null)
            return Result.Failure("Can't find module");

        module.UModule(request.Name, _current.UserId);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Module Updated Successfully");
    }
}
