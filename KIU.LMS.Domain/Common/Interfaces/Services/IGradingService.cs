namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IGradingService
{
    Task<(bool success, string message)> GradeSubmissionAsync(Solution solution);

}
