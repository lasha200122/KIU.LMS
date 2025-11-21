using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Voting;

public sealed record CreateVotingCommand(
    string Name, DateTimeOffset EndTime) : IRequest<Result<Guid>>;

public sealed class CreateVotingCommandValidator : AbstractValidator<CreateVotingCommand>
{
    public CreateVotingCommandValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Name).Length(1, 100);
        RuleFor(x => x.EndTime).LessThan(DateTimeOffset.UtcNow);
    }
}

public sealed class CreateVotingHandler(
    IValidator<CreateVotingCommand> validator,
    IVotingSessionRepository repository, 
    IUnitOfWork unitOfWork,
    ICurrentUserService userService) : IRequestHandler<CreateVotingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateVotingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<Guid>.Failure($"Validation errors: {validationResult.Errors.Select(x => x.ErrorMessage).ToList()}");
        
        var votingSession = new VotingSession(
            Guid.NewGuid(), userService.UserId, request.Name, request.EndTime);

        await repository.AddAsync(votingSession);

        await unitOfWork.SaveChangesAsync();
        
        return Result<Guid>.Success(votingSession.Id);
    }
}