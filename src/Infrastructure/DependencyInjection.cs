using Abstractions.Service;
using Ardalis.GuardClauses;
using LOLTierList.Infrastructure.Options;
using LOLTierList.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace LOLTierList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration cfg)
    {
        var ropts = new RedisClientOptions();
        cfg.GetSection("Redis").Bind(ropts);

        Guard.Against.NullOrWhiteSpace(ropts.ConnectionString, nameof(ropts.ConnectionString));

        s.AddSingleton(ropts);

        s.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var opt = ConfigurationOptions.Parse(ropts.ConnectionString);
            opt.AbortOnConnectFail = ropts.AbortOnConnectFail;
            opt.ResolveDns = ropts.ResolveDns;
            opt.ConnectRetry = ropts.ConnectRetry;
            opt.ConnectTimeout = ropts.ConnectTimeoutMs;
            opt.KeepAlive = ropts.KeepAliveSec;
            opt.DefaultDatabase = ropts.Database;
            return ConnectionMultiplexer.Connect(opt);
        });

        s.AddSingleton<IRedisService, RedisService>();

        s.AddHealthChecks().AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>().Configuration, "redis");

        return s;
    }
}
