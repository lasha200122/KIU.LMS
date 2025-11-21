using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Application.Features.Voting.Queries;

public sealed record GetVotingSessionDetailQuery(Guid Id) 
    : IRequest<Result<GetVotingSessionDetailQueryResponse>>;

public sealed record VotingOptionDetailDto(
    Guid OptionId, 
    string ImageUrl, 
    Guid FileRecordId,
    int Votes
);
public sealed record GetVotingSessionDetailQueryResponse(
    Guid Id, 
    string Name, 
    bool IsActive, 
    DateTimeOffset EndTime,
    IEnumerable<VotingOptionDetailDto> Options
);

public class GetVotingSessionDetailQueryHandler(
    IVotingSessionRepository votingSessionRepository) 
    : IRequestHandler<GetVotingSessionDetailQuery, Result<GetVotingSessionDetailQueryResponse>>
{
    public async Task<Result<GetVotingSessionDetailQueryResponse>> Handle(
        GetVotingSessionDetailQuery request, 
        CancellationToken cancellationToken)
    {
        var sessionDto = await votingSessionRepository.AsQueryable()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetVotingSessionDetailQueryResponse(
                x.Id,
                x.Name,
                x.IsActive,
                x.EndTime,
                x.Options.Select(o => new VotingOptionDetailDto(
                    o.Id,
                    $"/files/{o.Id}{Path.GetExtension(o.FileRecord.FileName)}",
                    o.FileRecordId,
                    o.Votes.Count
                )).ToList() 
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return sessionDto is null 
            ? Result<GetVotingSessionDetailQueryResponse>.Failure("Voting session not found")
            : Result<GetVotingSessionDetailQueryResponse>.Success(sessionDto);
    }
}
