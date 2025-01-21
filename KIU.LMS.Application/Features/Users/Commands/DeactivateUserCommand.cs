namespace KIU.LMS.Application.Features.Users.Commands;

public sealed record class DeactivateUserCommand(Guid Id) : IRequest<Result>;

public class DeactivateUserCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _current) : IRequestHandler<DeactivateUserCommand, Result>
{
    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.SingleOrDefaultWithTrackingAsync(x => x.Id == request.Id);

        if (user is null)
            return Result.Failure("Can't find user");

        if (user.IsDeleted)
            return Result.Failure($"{user.FirstName} {user.LastName} is already deactivated");

        user.Delete(_current.UserId, DateTimeOffset.UtcNow);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success($"{user.FirstName} {user.LastName} Deactivated successfully");
    }
}