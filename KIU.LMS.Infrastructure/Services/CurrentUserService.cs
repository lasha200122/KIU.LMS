namespace KIU.LMS.Infrastructure.Services;

public class CurrentUserService(
    IHttpContextAccessor _httpContextAccessor,
    ISafeExamBrowserService _sebService) : ICurrentUserService
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

    // Safe Exam Browser Detection
    public bool IsUsingSafeExamBrowser
    {
        get
        {
            if (_httpContextAccessor.HttpContext == null)
                return false;
            return _sebService.IsRequestFromSafeBrowser(_httpContextAccessor.HttpContext);
        }
    }

    public string? SebRequestHash
    {
        get
        {
            if (_httpContextAccessor.HttpContext == null)
                return null;
            var (requestHash, _) = _sebService.GetSebHeaders(_httpContextAccessor.HttpContext);
            return requestHash;
        }
    }

    public string? SebConfigKeyHash
    {
        get
        {
            if (_httpContextAccessor.HttpContext == null)
                return null;
            var (_, configKeyHash) = _sebService.GetSebHeaders(_httpContextAccessor.HttpContext);
            return configKeyHash;
        }
    }

    private string GetClaim(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
    }
}
