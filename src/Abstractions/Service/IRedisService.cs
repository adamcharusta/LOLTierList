namespace Abstractions.Service;

public interface IRedisService
{
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default);
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task<bool> RemoveAsync(string key, CancellationToken ct = default);
}
