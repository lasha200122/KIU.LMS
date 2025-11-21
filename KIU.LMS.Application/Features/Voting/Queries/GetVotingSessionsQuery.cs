using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Voting.Queries;
public sealed record GetVotingSessionsQuery(
    string? Name, 
    bool? IsActive, 
    int PageNumber, 
    int PageSize
) : IRequest<Result<PagedEntities<GetVotingSessionsQueryResponse>>>;

public sealed record GetVotingSessionsQueryResponse(
    Guid Id, 
    string Name, 
    bool IsActive, 
    DateTimeOffset EndTime, 
    int OptionsCount
);

public class GetVotingSessionsQueryValidator : AbstractValidator<GetVotingSessionsQuery>
{
    public GetVotingSessionsQueryValidator()
    {
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetVotingSessionsQueryHandler(
    IVotingSessionRepository votingSessionRepository) 
    : IRequestHandler<GetVotingSessionsQuery, Result<PagedEntities<GetVotingSessionsQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetVotingSessionsQueryResponse>>> Handle(
        GetVotingSessionsQuery request, 
        CancellationToken cancellationToken)
    {
        var sessions = await votingSessionRepository.GetPaginatedWhereMappedAsync(
            x => (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)) &&
                 (!request.IsActive.HasValue || x.IsActive == request.IsActive.Value),
            
            request.PageNumber,
            request.PageSize,
            
            x => new GetVotingSessionsQueryResponse(
                x.Id, 
                x.Name, 
                x.IsActive, 
                x.EndTime,
                x.Options.Count 
            ),
            x => x.Name,
            cancellationToken);

        return Result<PagedEntities<GetVotingSessionsQueryResponse>>.Success(sessions)!;
    }
}
