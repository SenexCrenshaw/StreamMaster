using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

namespace StreamMasterInfrastructure.Services;

public class BroadcastService(IHubContext<StreamMasterHub, IStreamMasterHub> hub, IClientStreamerManager clientStreamer, IStreamManager streamManager, IChannelService channelService, IStreamStatisticService streamStatisticService, ILogger<BroadcastService> logger) : IBroadcastService, IDisposable
{
    private Timer? _broadcastTimer;

    public void LogDebug()
    {

        if (channelService.GetGlobalStreamsCount() != 0)
        {
            logger.LogInformation("Global: {GetGlobalStreamsCount}", channelService.GetGlobalStreamsCount());
        }

        //logger.LogInformation("GetStreamHandlers: {GetStreamHandlers}", streamManager.GetStreamHandlers().Count);
        foreach (IClientStreamerConfiguration clientStreamerConfiguration in clientStreamer.GetAllClientStreamerConfigurations)
        {
            logger.LogInformation("Client: {ChannelName} {ReadBuffer.Id}", clientStreamerConfiguration.ChannelName, clientStreamerConfiguration.ReadBuffer?.Id ?? Guid.Empty);
        }

        foreach (IStreamHandler handler in streamManager.GetStreamHandlers())
        {
            logger.LogInformation("Stream: {count} {CircularRingBuffer} {VideoStreamName} {StreamUrl}", handler.ClientCount, handler.CircularRingBuffer.Id, handler.VideoStreamName, handler.StreamUrl);
        }
    }


    public void StartBroadcasting()
    {
        _broadcastTimer ??= new Timer(BroadcastMessage, null, 1000, 1000);
    }

    public void StopBroadcasting()
    {
        _broadcastTimer?.Dispose();
    }

    private bool sentEmpty = false;
    private void BroadcastMessage(object? state)
    {
        try
        {
            LogDebug();
            List<StreamStatisticsResult> statisticsResults = streamStatisticService.GetAllStatisticsForAllUrls().Result;
            if (statisticsResults.Any())
            {
                hub.Clients.All.StreamStatisticsResultsUpdate(statisticsResults).ConfigureAwait(false);
                sentEmpty = false;
            }
            else
            {
                if (!sentEmpty)
                {
                    hub.Clients.All.StreamStatisticsResultsUpdate(statisticsResults);
                }
                sentEmpty = true;
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