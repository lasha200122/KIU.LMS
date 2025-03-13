
namespace KIU.LMS.Application.Features.Modules.Commands;

public sealed record DeleteModuleCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteModuleCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<DeleteModuleCommand, Result>
{
    public async Task<Result> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await _unitOfWork.ModuleRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (module is null)
            return Result.Failure("Can't find module");

        module.Delete(_current.UserId, DateTimeOffset.UtcNow);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success("Module Deleted Successfully");
    }
}
