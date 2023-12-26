using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirect.Domain.Interfaces;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Services;

using System.Diagnostics;

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
    private readonly ISchedulesDirectDataService schedulesDirectDataService;
    private Timer? _broadcastTimer;

    public BroadcastService(IHubContext<StreamMasterHub, IStreamMasterHub> hub, IFileLoggingServiceFactory factory, ISchedulesDirectDataService schedulesDirectDataService, IStatisticsManager statisticsManager, IClientStreamerManager clientStreamer, IStreamManager streamManager, IChannelService channelService, IStreamStatisticService streamStatisticService, ILogger<BroadcastService> logger)
    {
        this.hub = hub;
        this.statisticsManager = statisticsManager;
        this.clientStreamer = clientStreamer;
        this.streamManager = streamManager;
        this.channelService = channelService;
        this.streamStatisticService = streamStatisticService;
        this.logger = logger;
        this.schedulesDirectDataService = schedulesDirectDataService;
        debugLogger = factory.Create("FileLoggerDebug");
    }

    private void printDebug(string format, params object[] args)
    {
        string formattedMessage = string.Format(format, args);
        Debug.WriteLine(formattedMessage);
        //debugLogger.EnqueueLogEntry(formattedMessage);
    }
    public void LogDebug()
    {
        if (schedulesDirectDataService.SchedulesDirectDatas.Any())
        {
            printDebug("SchedulesDirectDatas: {0}", schedulesDirectDataService.SchedulesDirectDatas.Count);
            foreach (KeyValuePair<int, ISchedulesDirectData> sd in schedulesDirectDataService.SchedulesDirectDatas)
            {
                printDebug("SchedulesDirectData: {0} {1} {2}", sd.Key, sd.Value.Services.Count, sd.Value.Programs.Count);
            }
        }

        if (statisticsManager.GetAllClientIds().Any())
        {
            printDebug("Stat ClientIds: {0}", statisticsManager.GetAllClientIds().Count);
        }
        if (channelService.GetGlobalStreamsCount() != 0)
        {
            printDebug("Global: {0}", channelService.GetGlobalStreamsCount());
        }

        if (channelService.GetChannelStatuses().Any())
        {
            printDebug("GetChannelStatuses: {0}", channelService.GetChannelStatuses().Count);
        }

        //logger.LogInformation("GetStreamHandlers: {GetStreamHandlers}", streamManager.GetStreamHandlers().Count);
        foreach (IClientStreamerConfiguration clientStreamerConfiguration in clientStreamer.GetAllClientStreamerConfigurations)
        {
            printDebug("Client: {0} {1}", clientStreamerConfiguration.ChannelName, clientStreamerConfiguration.ReadBuffer?.Id ?? Guid.Empty);
        }

        foreach (IStreamHandler handler in streamManager.GetStreamHandlers())
        {
            printDebug("Stream: {0} {1} {2} {3}", handler.ClientCount, handler.CircularRingBuffer.Id, handler.VideoStreamName, handler.StreamUrl);
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