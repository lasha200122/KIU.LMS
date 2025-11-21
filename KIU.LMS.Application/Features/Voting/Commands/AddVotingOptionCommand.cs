using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL;
using Microsoft.AspNetCore.Http;

namespace KIU.LMS.Application.Features.Voting.Commands;

public sealed record AddVotingOptionCommand(
    Guid VoteSessionId,
    IFormFile File) : IRequest<Result<Guid>>;

public class AddVotingOptionCommandHandler(
    IVotingSessionRepository votingRepository,
    IVotingOptionRepository votingOptionRepository,
    IFileService fileService,
    IUnitOfWork unitOfWork,
    ICurrentUserService userService)
    : IRequestHandler<AddVotingOptionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddVotingOptionCommand request, CancellationToken cancellationToken)
    {
        var session = await votingRepository.FirstOrDefaultAsync(x => x.Id == request.VoteSessionId, cancellationToken);

        var optionId = Guid.NewGuid();
        
        if (session is null)
            return Result<Guid>.Failure("Voting session not found");

        if (!session.IsActive) 
            return Result<Guid>.Failure("Cannot add options to a closed session");

        var fileRecord = await fileService.UploadFileAsync(
            objectId: session.Id.ToString(),
            objectType: "VotingSession",
            file: request.File,
            optionId.ToString()
        );

        try
        {
            var newOption = new VotingOption(optionId, session.Id, fileRecord.Id,
                userService.UserId);
            await votingOptionRepository.AddAsync(newOption);
            
            await unitOfWork.SaveChangesAsync();

            var newOptionId = newOption.Id; 
            return Result<Guid>.Success(newOptionId);
        }
        catch (Exception ex)
        {
            await fileService.DeleteFileAsync(fileRecord.Id);
            
            return Result<Guid>.Failure($"Failed to create voting option: {ex.Message}");
        }
    }
}