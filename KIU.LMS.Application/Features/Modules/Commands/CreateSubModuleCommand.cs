namespace KIU.LMS.Application.Features.Modules.Commands;

public sealed record CreateSubModuleCommand(
        Guid ModuleId,
        string Name,
        string? Problem,
        string? Code,
        SubModuleType Type) : IRequest<Result>;

public sealed class CreateSubModuleCommandValidator : AbstractValidator<CreateSubModuleCommand> 
{
    public CreateSubModuleCommandValidator() 
    {
        RuleFor(x => x.ModuleId)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull();
    }
}

public sealed class CreateSubModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<CreateSubModuleCommand, Result>
{
    public async Task<Result> Handle(CreateSubModuleCommand request, CancellationToken cancellationToken)
    {
        var subModule = new SubModule(
            Guid.NewGuid(),
            request.ModuleId,
            request.Name,
            request.Problem,
            request.Code,
            request.Type,
            _current.UserId);

        await _unitOfWork.SubModuleRepository.AddAsync(subModule);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Sub module created successfully");
    }
}
