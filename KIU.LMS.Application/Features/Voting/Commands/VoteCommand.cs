using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Voting.Commands;

public sealed record VoteCommand(
    Guid VoteSessionId,
    Guid OptionId
) : IRequest<Result<Guid>>;


public class CastVoteCommandHandler(
    IVotingSessionRepository sessionRepository,
    IVoteRepository voteRepository, 
    IUnitOfWork unitOfWork,
    ICurrentUserService userService)
    : IRequestHandler<VoteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(VoteCommand request, CancellationToken cancellationToken)
    {
        var userId = userService.UserId;

        var session = await sessionRepository
            .GetByIdWithOptionsAsync(request.VoteSessionId, cancellationToken);

        if (session is null)
            return Result<Guid>.Failure("Voting session not found");

        if (!session.IsActive)
            return Result<Guid>.Failure("Voting session is closed");

        if (DateTimeOffset.UtcNow > session.EndTime)
            return Result<Guid>.Failure("Voting period has ended");

        var isOptionValid = session.Options.Any(o => o.Id == request.OptionId);
        if (!isOptionValid)
            return Result<Guid>.Failure("Invalid option selected for this session");

        var hasAlreadyVoted = await voteRepository.HasUserVotedAsync(
            request.VoteSessionId, 
            userId, 
            cancellationToken);

        if (hasAlreadyVoted)
            return Result<Guid>.Failure("You have already voted in this session");

        var vote = new Vote(
            Guid.NewGuid(),
            request.VoteSessionId,
            request.OptionId,
            userId
        );

        try
        {
            await voteRepository.AddAsync(vote, cancellationToken);
            await unitOfWork.SaveChangesAsync();
            
            return Result<Guid>.Success(vote.Id);
        }
        catch (DbUpdateException)
        {
            return Result<Guid>.Failure("You have already voted in this session");
        }
    }
}