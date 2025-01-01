namespace KIU.LMS.Domain.Common.Settings.Redis;

public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int SessionTimeout { get; set; }
    public string Password { get; set; } = string.Empty;
}
