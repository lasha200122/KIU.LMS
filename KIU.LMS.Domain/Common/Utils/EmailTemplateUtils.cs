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
}
