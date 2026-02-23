namespace KIU.LMS.Domain.Common.Models.SafeExamBrowser;

public sealed class SebConfiguration
{
    public string QuizId { get; }
    public string StartUrl { get; }
    public string BrowserExamKey { get; }
    public bool AllowQuit { get; }
    public bool EnableSwitchToApplications { get; }
    public bool AllowSpellCheck { get; }
    public bool ShowTaskBar { get; }
    public int QuitUrlConfirmation { get; }

    public SebConfiguration(
        string quizId,
        string baseUrl,
        string browserExamKey,
        bool allowQuit = false,
        bool enableSwitchToApplications = false,
        bool allowSpellCheck = false,
        bool showTaskBar = false,
        int quitUrlConfirmation = 1)
    {
        if (string.IsNullOrWhiteSpace(quizId))
            throw new ArgumentException("Quiz ID is required", nameof(quizId));
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL is required", nameof(baseUrl));
        if (string.IsNullOrWhiteSpace(browserExamKey))
            throw new ArgumentException("Browser Exam Key is required", nameof(browserExamKey));

        QuizId = quizId;
        StartUrl = $"{baseUrl.TrimEnd('/')}/exam/{quizId}";
        BrowserExamKey = browserExamKey;
        AllowQuit = allowQuit;
        EnableSwitchToApplications = enableSwitchToApplications;
        AllowSpellCheck = allowSpellCheck;
        ShowTaskBar = showTaskBar;
        QuitUrlConfirmation = quitUrlConfirmation;
    }
}
