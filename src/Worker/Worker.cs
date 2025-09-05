using LOLTierList.Abstractions.Services;
using Serilog;

namespace LOLTierList.Worker;

public class Worker(IRedisService redisService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("Worker running at: {time}", DateTimeOffset.Now);

            await redisService.SetAsync("TestInfo", new { Time = DateTimeOffset.Now }, TimeSpan.FromMinutes(5),
                stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
