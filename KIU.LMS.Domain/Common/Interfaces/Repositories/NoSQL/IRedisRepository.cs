namespace KIU.LMS.Domain.Common.Interfaces.Repositories.NoSQL;

public interface IRedisRepository<T> where T : class
{
    Task<T?> GetAsync(string key);
    Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern);
    Task<IEnumerable<T>> GetAllAsync();
    Task<bool> SetAsync(string key, T value, TimeSpan? expiry = null);
    Task<bool> DeleteAsync(string key);
    Task<bool> ExistsAsync(string key);
}