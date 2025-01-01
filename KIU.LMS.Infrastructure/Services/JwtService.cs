namespace KIU.LMS.Infrastructure.Services;

public class JwtService(JwtSettings _settings) : IJwtService
{
    public string GenerateJwtToken(User user)
    {
        var signingCredinals = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Email,user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, nameof(user.Role)),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expires = DateTimeOffset.UtcNow.AddMinutes(_settings.ExpiryMinutes).UtcDateTime;

        var securityToken = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            expires: expires,
            claims: claims,
            signingCredentials: signingCredinals);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public RefreshToken GenerateRefreshToken()
    {
        _ = int.TryParse(_settings.RefreshTokenValidityInDays.ToString(), out int refreshTokenValidityInDays);

        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        RefreshToken result = new(Convert.ToBase64String(randomNumber), DateTimeOffset.Now.AddDays(refreshTokenValidityInDays));

        return result;
    }
}
