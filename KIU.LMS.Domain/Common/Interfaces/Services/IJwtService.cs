namespace KIU.LMS.Domain.Common.Interfaces.Services;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    RefreshToken GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
