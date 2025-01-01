namespace KIU.LMS.Domain.Common.Models.User;

public sealed record UserDto(
    string FullName,
    string Email,
    string Role);