using System.Text.Json;

namespace KIU.LMS.Domain.Common.Utils;

public static class EmailTemplateUtils
{
    public static string RegisterUserVariableBuilder(User user, string url) 
    {
        var variables = new Dictionary<string, string>
        {
            { "fullname", $"{user.FirstName} {user.LastName}" },
            { "email", user.Email },
            { "url", url }
        };

        return JsonSerializer.Serialize(variables);
    }

    public static string ResetPasswordVariableBuilder(User user, string url)
    {
        var variables = new Dictionary<string, string>
        {
            { "fullname", $"{user.FirstName} {user.LastName}" },
            { "email", user.Email },
            { "url", url }
        };

        return JsonSerializer.Serialize(variables);
    }

    public static string MeetingCreatedVariableBuilder(User user, CourseMeeting meeting, string courseName) 
    {
        var variables = new Dictionary<string, string> 
        {
            { "fullname", $"{user.FirstName} {user.LastName}" },
            { "courseName", $"{courseName}"},
            { "meetingName", $"{meeting.Name}" },
            { "startDate", $"{meeting.StartTime.ToString("dd, MMMM, yyyy")}" },
            { "startTime", $"{meeting.StartTime.ToString("HH:mm")}" },
            { "endTime", $"{meeting.EndTime.ToString("HH:mm")}"},
            { "meetingLink", $"{meeting.Url}"}
        };

        return JsonSerializer.Serialize(variables);
    }
}
