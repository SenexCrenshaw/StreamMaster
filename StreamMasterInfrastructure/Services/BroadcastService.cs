using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

namespace StreamMasterInfrastructure.Services;

public class BroadcastService(IHubContext<StreamMasterHub, IStreamMasterHub> hub, IChannelService channelService, IStreamManager streamManager, IStreamStatisticService streamStatisticService, ILogger<BroadcastService> logger) : IBroadcastService, IDisposable
{
    private Timer? _broadcastTimer;

    public void LogDebug()
    {
        logger.LogInformation("ChannelManager LogDebug");
        logger.LogInformation("GetGlobalStreamsCount: {GetGlobalStreamsCount}", channelService.GetGlobalStreamsCount());
        logger.LogInformation("GetStreamHandlers: {GetStreamHandlers}", streamManager.GetStreamHandlers().Count);
        logger.LogInformation("GetStreamInformations: {GetStreamInformations}", streamManager.GetStreamInformations().Count);
    }


    public void StartBroadcasting()
    {
        _broadcastTimer ??= new Timer(BroadcastMessage, null, 1000, 1000);
    }

    public void StopBroadcasting()
    {
        _broadcastTimer?.Dispose();
    }

    private void BroadcastMessage(object? state)
    {
        try
        {
            LogDebug();
            List<StreamStatisticsResult> statisticsResults = streamStatisticService.GetAllStatisticsForAllUrls().Result;
            if (statisticsResults.Any())
            {
                hub.Clients.All.StreamStatisticsResultsUpdate(statisticsResults).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Error while broadcasting message: {Message}", ex.Message);
        }
    }

    public void Dispose()
    {
        StopBroadcasting();
        _broadcastTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}