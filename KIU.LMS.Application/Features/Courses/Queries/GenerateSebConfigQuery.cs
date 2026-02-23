using KIU.LMS.Domain.Common.Models.SafeExamBrowser;

namespace KIU.LMS.Application.Features.Courses.Queries;

public sealed record GenerateSebConfigQuery(Guid QuizId) : IRequest<Result<GenerateSebConfigQueryResponse>>;

public sealed record GenerateSebConfigQueryResponse(
    byte[] FileContent,
    string FileName,
    string ContentType);

public class GenerateSebConfigQueryValidator : AbstractValidator<GenerateSebConfigQuery>
{
    public GenerateSebConfigQueryValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("Quiz ID is required");
    }
}

public class GenerateSebConfigQueryHandler(
    IUnitOfWork _unitOfWork,
    ISafeExamBrowserService _sebService,
    IConfiguration _configuration) : IRequestHandler<GenerateSebConfigQuery, Result<GenerateSebConfigQueryResponse>>
{
    public async Task<Result<GenerateSebConfigQueryResponse>> Handle(
        GenerateSebConfigQuery request,
        CancellationToken cancellationToken)
    {
        var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(request.QuizId);

        if (quiz is null)
            return Result<GenerateSebConfigQueryResponse>.Failure("Quiz not found");

        if (!quiz.RequiresSafeExamBrowser)
            return Result<GenerateSebConfigQueryResponse>.Failure("This quiz does not require Safe Exam Browser");

        if (string.IsNullOrWhiteSpace(quiz.SafeExamBrowserConfigKey))
            return Result<GenerateSebConfigQueryResponse>.Failure("SEB configuration not generated");

        // Get frontend URL from configuration
        var baseUrl = _configuration["AppSettings:FrontendUrl"] ?? "https://learn-with-ai.kiu.edu.ge";

        var sebConfig = new SebConfiguration(
            quiz.Id.ToString(),
            baseUrl,
            quiz.SafeExamBrowserConfigKey,
            allowQuit: false,
            enableSwitchToApplications: false,
            allowSpellCheck: false,
            showTaskBar: false,
            quitUrlConfirmation: 1);

        var configBytes = _sebService.GenerateConfigurationFile(sebConfig);
        var fileName = $"{quiz.Title.Replace(" ", "_")}_SEB.seb";

        var response = new GenerateSebConfigQueryResponse(
            configBytes,
            fileName,
            "application/x-sebconfig");

        return Result<GenerateSebConfigQueryResponse>.Success(response);
    }
}
