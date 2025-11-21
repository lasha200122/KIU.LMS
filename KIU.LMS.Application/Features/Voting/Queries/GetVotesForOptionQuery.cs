using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;

namespace KIU.LMS.Application.Features.Voting.Queries;

public sealed record GetVotesForOptionQuery(
    Guid OptionId, 
    int PageNumber, 
    int PageSize
) : IRequest<Result<PagedEntities<GetVotesForOptionQueryResponse>>>;

public sealed record GetVotesForOptionQueryResponse(
    Guid UserId, 
    string UserFullName, 
    DateTimeOffset VotedAt
);

public class GetVotesForOptionQueryValidator : AbstractValidator<GetVotesForOptionQuery>
{
    public GetVotesForOptionQueryValidator()
    {
        RuleFor(p => p.OptionId).NotEmpty();
        RuleFor(p => p.PageNumber).GreaterThan(0);
        RuleFor(p => p.PageSize).GreaterThan(0);
    }
}

public class GetVotesForOptionQueryHandler(
    IVoteRepository voteRepository) 
    : IRequestHandler<GetVotesForOptionQuery, Result<PagedEntities<GetVotesForOptionQueryResponse>>>
{
    public async Task<Result<PagedEntities<GetVotesForOptionQueryResponse>>> Handle(
        GetVotesForOptionQuery request, 
        CancellationToken cancellationToken)
    {
        var votes = await voteRepository.GetPaginatedWhereMappedAsync(
            x => x.OptionId == request.OptionId,
            
            request.PageNumber,
            request.PageSize,
            
            x => new GetVotesForOptionQueryResponse(
                x.UserId, 
                x.User.FirstName + x.User.LastName ?? "Unknown User", 
                x.CreateDate 
            ),
            
            x => x.CreateDate, 
            
            cancellationToken);

        return Result<PagedEntities<GetVotesForOptionQueryResponse>>.Success(votes)!;
    }
}
