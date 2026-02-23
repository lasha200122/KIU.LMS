namespace KIU.LMS.Application.Features.QuizSessions.Commands;

public sealed record StartSessionCommand(Guid Id) : IRequest<Result<string>>;


public class StartSessionCommandValidator : AbstractValidator<StartSessionCommand> 
{
    public StartSessionCommandValidator() 
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}


public class StartSessionCommandHandler(
    IExamService _service,
    ICurrentUserService _current,
    IUnitOfWork _unitOfWork,
    ISafeExamBrowserService _sebService) : IRequestHandler<StartSessionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(StartSessionCommand request, CancellationToken cancellationToken)
    {
        // Check if quiz requires Safe Exam Browser
        var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(request.Id);

        if (quiz == null)
            return Result<string>.Failure("Quiz not found");

        // Check SEB requirement
        if (quiz.RequiresSafeExamBrowser && !_current.IsUsingSafeExamBrowser)
            return Result<string>.Failure("This quiz requires Safe Exam Browser. Please use SEB to start the session.");

        // Validate SEB headers if required
        if (quiz.RequiresSafeExamBrowser)
        {
            var validation = _sebService.ValidateRequest(
                _current.SebRequestHash ?? "",
                _current.SebConfigKeyHash ?? "",
                quiz.SafeExamBrowserConfigKey ?? "",
                $"/api/quiz/sessions/{request.Id}/start",
                "POST");

            if (!validation.IsValid)
                return Result<string>.Failure($"SEB validation failed: {validation.ErrorMessage}");
        }

        var result = await _service.StartExamAsync(_current.UserId, request.Id);

        if (result is null)
            return Result<string>.Failure("Can't start Session");

        return Result<string>.Success(result.Id);
    }
}
