using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL.Base;

namespace KIU.LMS.Application.Features.Modules.Commands;


public sealed record DeleteModuleBankCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteSubModuleCommandValidator : AbstractValidator<DeleteModuleBankCommand>
{
    public DeleteSubModuleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}

public sealed class DeleteSubModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<DeleteModuleBankCommand, Result>
{
    public async Task<Result> Handle(DeleteModuleBankCommand request, CancellationToken cancellationToken)
    {
        var sub = await _unitOfWork.ModuleBankRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (sub is null)
            return Result.Failure("Can't find sub module");

        sub.Delete(_current.UserId, DateTimeOffset.UtcNow);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Sub module deleted successfully");
    }
}



// New Version

public sealed record DeleteSubModuleCommand(Guid Id) : IRequest<Result>;

public class DeleteSubModulesCommandValidator : AbstractValidator<DeleteSubModuleCommand>
{
    public DeleteSubModulesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("SubModule Id is required");
    }
}

public sealed class DeleteSubModulesCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService
) : IRequestHandler<DeleteSubModuleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteSubModuleCommand request, CancellationToken cancellationToken)
    {
        var subModule = await _unitOfWork.SubModuleRepository.SingleOrDefaultAsync(x => x.Id == request.Id);

        if (subModule == null)
        {
            return Result.Failure("SubModule not found");
        }

        subModule.Delete(currentUserService.UserId, DateTimeOffset.UtcNow);

        _unitOfWork.SubModuleRepository.Update(subModule);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success("SubModule deleted successfully");
    }
}