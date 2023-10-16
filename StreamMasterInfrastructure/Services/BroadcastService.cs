using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

namespace StreamMasterInfrastructure.Services;

public class BroadcastService : IBroadcastService, IDisposable
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hub;
    private Timer? _broadcastTimer;
    private readonly ILogger<BroadcastService> _logger;
    private readonly IStreamStatisticService _streamStatisticService;

    public BroadcastService(IHubContext<StreamMasterHub, IStreamMasterHub> hub, IStreamStatisticService streamStatisticService, ILogger<BroadcastService> logger)
    {
        _hub = hub;
        _logger = logger;
        _streamStatisticService = streamStatisticService;

        StartBroadcasting();
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
            List<StreamStatisticsResult> statisticsResults = _streamStatisticService.GetAllStatisticsForAllUrls().Result;
            _hub.Clients.All.StreamStatisticsResultsUpdate(statisticsResults).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while broadcasting message: {Message}", ex.Message);
        }
    }


    public void Dispose()
    {
        StopBroadcasting();
        _broadcastTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}