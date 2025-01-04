namespace KIU.LMS.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor _httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var claim = GetClaim(ClaimTypes.NameIdentifier);
            return Guid.Parse(claim);
        }
    }

    public string Email => GetClaim(ClaimTypes.Email);
    public string FullName => GetClaim(ClaimTypes.Name);
    public string Role => GetClaim(ClaimTypes.Role);
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    public string? DeviceId => "TEST ID";//_httpContextAccessor.HttpContext?.Request.Headers["X-Device-Info"].FirstOrDefault();

    private string GetClaim(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
    }
}
