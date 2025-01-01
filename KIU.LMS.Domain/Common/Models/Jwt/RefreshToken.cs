namespace KIU.LMS.Domain.Common.Models.Jwt;

public sealed record RefreshToken(string Token, DateTimeOffset Expires);