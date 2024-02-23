using Microsoft.Extensions.Hosting;

using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Services;

public class InactiveStreamMonitor(IVideoStreamService videoStreamService, IHLSManager hLsManager, IAccessTracker accessTracker) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            List<string> inactiveStreams = accessTracker.GetInactiveStreams().ToList();
            foreach (string? videoStreamId in inactiveStreams)
            {
                accessTracker.Remove(videoStreamId);

                hLsManager.Stop(videoStreamId);

                videoStreamService.RemoveVideoStreamDto(videoStreamId);
                //IHLSHandler? hls = _hLsManager.Get(videoStreamId);
                //if (hls is null)
                //{
                //    continue;
                //}
                //hls.Stop();

            }

            await Task.Delay(accessTracker.CheckInterval, stoppingToken);
        }
    }
}
