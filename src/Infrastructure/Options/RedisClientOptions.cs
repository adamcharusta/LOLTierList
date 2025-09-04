namespace LOLTierList.Infrastructure.Options;

public sealed class RedisClientOptions
{
    public string ConnectionString { get; init; } = "redis:6379";
    public bool AbortOnConnectFail { get; init; } = false;
    public bool ResolveDns { get; init; } = true;
    public int ConnectRetry { get; init; } = 3;
    public int ConnectTimeoutMs { get; init; } = 5000;
    public int KeepAliveSec { get; init; } = 60;
    public int Database { get; init; } = 0;
    public string KeyPrefix { get; init; } = "loltierlist:";
}
