namespace KIU.LMS.Domain.Common.Utils;

public static class EnumTranslator
{
    public static string UserRoles(UserRole role) => role switch 
    {
        UserRole.Admin => "Admin",
        UserRole.Student => "Student",
        UserRole.Ta => "Ta",
        UserRole.Professor => "Professor",
        _ => ""
    };
}
