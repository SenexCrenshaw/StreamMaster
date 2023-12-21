using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Services;

namespace StreamMasterInfrastructure.Services;

public class BroadcastService : IBroadcastService, IDisposable
{
    private readonly IFileLoggingService debugLogger;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> hub;
    private readonly IStatisticsManager statisticsManager;
    private readonly IClientStreamerManager clientStreamer;
    private readonly IStreamManager streamManager;
    private readonly IChannelService channelService;
    private readonly IStreamStatisticService streamStatisticService;
    private readonly ILogger<BroadcastService> logger;
    private Timer? _broadcastTimer;

    public BroadcastService(IHubContext<StreamMasterHub, IStreamMasterHub> hub, IFileLoggingServiceFactory factory, IStatisticsManager statisticsManager, IClientStreamerManager clientStreamer, IStreamManager streamManager, IChannelService channelService, IStreamStatisticService streamStatisticService, ILogger<BroadcastService> logger)
    {
        this.hub = hub;
        this.statisticsManager = statisticsManager;
        this.clientStreamer = clientStreamer;
        this.streamManager = streamManager;
        this.channelService = channelService;
        this.streamStatisticService = streamStatisticService;
        this.logger = logger;
        debugLogger = factory.Create("FileLoggerDebug");
    }


    public void LogDebug()
    {
        if (statisticsManager.GetAllClientIds().Any())
        {
            debugLogger.EnqueueLogEntry("Stat ClientIds: {0}", statisticsManager.GetAllClientIds().Count);
        }
        if (channelService.GetGlobalStreamsCount() != 0)
        {
            debugLogger.EnqueueLogEntry("Global: {0}", channelService.GetGlobalStreamsCount());
        }

        if (channelService.GetChannelStatuses().Any())
        {
            debugLogger.EnqueueLogEntry("GetChannelStatuses: {0}", channelService.GetChannelStatuses().Count);
        }

        //logger.LogInformation("GetStreamHandlers: {GetStreamHandlers}", streamManager.GetStreamHandlers().Count);
        foreach (IClientStreamerConfiguration clientStreamerConfiguration in clientStreamer.GetAllClientStreamerConfigurations)
        {
            debugLogger.EnqueueLogEntry("Client: {0} {1}", clientStreamerConfiguration.ChannelName, clientStreamerConfiguration.ReadBuffer?.Id ?? Guid.Empty);
        }

        foreach (IStreamHandler handler in streamManager.GetStreamHandlers())
        {
            debugLogger.EnqueueLogEntry("Stream: {0} {1} {2} {3}", handler.ClientCount, handler.CircularRingBuffer.Id, handler.VideoStreamName, handler.StreamUrl);
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
                //if (!sentEmpty)
                //{
                hub.Clients.All.StreamStatisticsResultsUpdate(statisticsResults).ConfigureAwait(false);
                //}
                //sentEmpty = true;
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