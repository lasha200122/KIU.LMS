namespace KIU.LMS.Application.Features.Courses.Commands;

public sealed record UpdateQuizSebSettingsCommand(
    Guid QuizId,
    bool RequiresSafeExamBrowser,
    bool RegenerateConfig = false) : IRequest<Result>;

public class UpdateQuizSebSettingsCommandValidator : AbstractValidator<UpdateQuizSebSettingsCommand>
{
    public UpdateQuizSebSettingsCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("Quiz ID is required");
    }
}

public class UpdateQuizSebSettingsCommandHandler(
    IUnitOfWork _unitOfWork,
    ISafeExamBrowserService _sebService,
    ICurrentUserService _current) : IRequestHandler<UpdateQuizSebSettingsCommand, Result>
{
    public async Task<Result> Handle(UpdateQuizSebSettingsCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(request.QuizId);

        if (quiz is null)
            return Result.Failure("Quiz not found");

        // Check if quiz has already started
        if (quiz.StartDateTime <= DateTimeOffset.UtcNow)
            return Result.Failure("Cannot modify SEB settings for a quiz that has already started");

        if (request.RequiresSafeExamBrowser)
        {
            if (!quiz.RequiresSafeExamBrowser || request.RegenerateConfig)
            {
                var configKey = _sebService.GenerateBrowserExamKey(quiz.Id, quiz.Id.ToString());

                if (quiz.RequiresSafeExamBrowser)
                    quiz.RegenerateSebConfig(configKey);
                else
                    quiz.EnableSafeExamBrowser(configKey);
            }
        }
        else
        {
            quiz.DisableSafeExamBrowser();
        }

        quiz.Update(_current.UserId, DateTimeOffset.UtcNow);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success("SEB settings updated successfully");
    }
}
