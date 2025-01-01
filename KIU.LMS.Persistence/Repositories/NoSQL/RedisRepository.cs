using StackExchange.Redis;

namespace KIU.LMS.Persistence.Repositories.NoSQL;

public class RedisRepository<T> : IRedisRepository<T> where T : class
{
    private readonly IConnectionMultiplexer _redis;
    private readonly StackExchange.Redis.IDatabase _database;
    private readonly string _prefix;

    public RedisRepository(IConnectionMultiplexer redis, string prefix = "")
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _prefix = prefix;
    }

    private string GetKey(string key) => string.IsNullOrEmpty(_prefix) ? key : $"{_prefix}:{key}";

    public async Task<T?> GetAsync(string key)
    {
        var value = await _database.StringGetAsync(GetKey(key));
        if (value.IsNull)
            return null;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task<IEnumerable<string>> GetKeysByPatternAsync(string pattern)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);
        return keys.Select(k => k.ToString());
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{_prefix}:*");
        var items = new List<T>();

        foreach (var key in keys)
        {
            var value = await _database.StringGetAsync(key);
            if (!value.IsNull)
            {
                items.Add(JsonSerializer.Deserialize<T>(value!)!);
            }
        }

        return items;
    }

    public async Task<bool> SetAsync(string key, T value, TimeSpan? expiry = null)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        return await _database.StringSetAsync(GetKey(key), serializedValue, expiry);
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _database.KeyDeleteAsync(GetKey(key));
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(GetKey(key));
    }
}