using Abstractions.Service;
using LOLTierList.Infrastructure.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace LOLTierList.Infrastructure.Services;

public class RedisService(IConnectionMultiplexer mux, RedisClientOptions opts) : IRedisService
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Converters = { new StringEnumConverter() }
    };

    private readonly IDatabase _db = mux.GetDatabase(opts.Database);
    private readonly string _prefix = opts.KeyPrefix;

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var json = JsonConvert.SerializeObject(value, JsonSettings);
        await _db.StringSetAsync(K(key), json, ttl);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var val = await _db.StringGetAsync(K(key));
        if (val.IsNullOrEmpty)
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(val!, JsonSettings);
    }

    public Task<bool> RemoveAsync(string key, CancellationToken ct = default)
    {
        return _db.KeyDeleteAsync(K(key));
    }

    private string K(string key)
    {
        return string.IsNullOrEmpty(_prefix) ? key : $"{_prefix}{key}";
    }
}
