namespace KIU.LMS.Application.Features.Modules.Commands;

public sealed record CreateModuleCommand(Guid CourseId, string Name) : IRequest<Result>;

public class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand> 
{
    public CreateModuleCommandValidator() 
    {
        RuleFor(x => x.CourseId)
            .NotNull();

        RuleFor(x => x.Name)
            .NotNull();
    }
}

public sealed class CreateModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<CreateModuleCommand, Result>
{
    public async Task<Result> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        var module = new Domain.Entities.SQL.Module(
            Guid.NewGuid(),
            request.CourseId,
            request.Name,
            _current.UserId);

        await _unitOfWork.ModuleRepository.AddAsync(module);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Module Created successfully");
    }
}
