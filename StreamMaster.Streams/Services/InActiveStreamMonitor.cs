using Microsoft.Extensions.Hosting;

namespace StreamMaster.Streams.Services;

public class InActiveStreamMonitor(ILogger<InActiveStreamMonitor> logger, IHLSManager hLsManager, IAccessTracker accessTracker) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            List<StreamAccessInfo> inactiveStreams = accessTracker.GetInactiveStreams().ToList();
            foreach (StreamAccessInfo streamAccessInfo in inactiveStreams)
            {
                accessTracker.RemoveAccessTime(streamAccessInfo.Key);
                if (streamAccessInfo.MillisecondsSinceLastUpdate > 0)
                {
                    logger.LogInformation("M3U8 last update {key} {Milliseconds}ms {InactiveThreshold}", streamAccessInfo.Key, streamAccessInfo.MillisecondsSinceLastUpdate, streamAccessInfo.InactiveThreshold);
                }
                hLsManager.Stop(streamAccessInfo.SMStreamId);
            }

            await Task.Delay(accessTracker.CheckInterval, stoppingToken);
        }
    }
}
