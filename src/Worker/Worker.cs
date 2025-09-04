using Abstractions.Service;

namespace LOLTierList.Worker;

public class Worker(ILogger<Worker> logger, IRedisService redisService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await redisService.SetAsync("TestInfo", new { Time = DateTimeOffset.Now }, TimeSpan.FromMinutes(5),
                stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
