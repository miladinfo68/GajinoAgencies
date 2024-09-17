using Newtonsoft.Json;
using StackExchange.Redis;

namespace GajinoAgencies.Services;

public interface ICacheManagerService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string pattern, string key, T value, TimeSpan? expiration = null);
    Task<bool> RemoveAsync(string key);
    //-----------------------------------------------------------------
    T? Get<T>(string key);
    bool Set<T>(string pattern, string key, T value, TimeSpan? expiration = null);
    bool Remove(string key);
}


public class CacheManagerService : ICacheManagerService
{
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _db;
    private readonly TimeSpan _defaultExpiration;


    private TimeSpan SetExpirationTime(TimeSpan? expirationTime = null) =>
        expirationTime ?? _defaultExpiration;

    public CacheManagerService(
        IConnectionMultiplexer connectionMultiplexer,
        TimeSpan defaultExpiration
        )
    {
        _connection = connectionMultiplexer;
        _db = _connection.GetDatabase();
        _defaultExpiration = defaultExpiration;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? JsonConvert.DeserializeObject<T>(value) : default;
    }
    
    public async Task<bool> SetAsync<T>(string pattern, string key, T value, TimeSpan? expiration = null)
    {
        var key1 = $"{pattern}{key}";
        return await _db.StringSetAsync(key1, JsonConvert.SerializeObject(value), SetExpirationTime(expiration));
    }

    
    public async Task<bool> RemoveAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }

    public T? Get<T>(string key)
    {
        var value = _db.StringGet(key);
        return value.HasValue ? JsonConvert.DeserializeObject<T>(value) : default;
    }

  
    public bool Set<T>(string pattern, string key, T value, TimeSpan? expiration = null)
    {
        var key1 = $"{pattern}{key}";
        return _db.StringSet(key1, JsonConvert.SerializeObject(value), SetExpirationTime(expiration));
    }

   
    public bool Remove(string key)
    {
        return _db.KeyDelete(key);
    }

    
}
