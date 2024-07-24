//using Microsoft.Extensions.Hosting;

//namespace StreamMaster.Infrastructure.Services;

//public class InactiveStreamMonitor(IVideoStreamService videoStreamService, IHLSManager hLsManager, IAccessTracker accessTracker) : BackgroundService
//{
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            List<int> inactiveStreams = accessTracker.GetInactiveStreams().ToList();
//            foreach (int smChannelId in inactiveStreams)
//            {
//                accessTracker.Remove(smChannelId);

//                hLsManager.Stop(smChannelId);

//                videoStreamService.RemoveVideoStreamDto(smChannelId);
//                //IHLSHandler? hls = _hLsManager.Get(videoStreamId);
//                //if (hls is null)
//                //{
//                //    continue;
//                //}
//                //hls.Stop();

//            }

//            await Task.Delay(accessTracker.CheckInterval, stoppingToken);
//        }
//    }
//}
