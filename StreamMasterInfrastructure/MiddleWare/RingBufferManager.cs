using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;

namespace StreamMasterInfrastructure.MiddleWare;

public class RingBufferManager : IDisposable, IRingBufferManager
{
    private readonly Timer _broadcastTimer;
    private readonly IChannelManager _channeManager;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hub;
    private readonly ILogger<RingBufferManager> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RingBufferManager(
        ILogger<RingBufferManager> logger,
        IServiceProvider serviceProvider,
        IHubContext<StreamMasterHub,
        IStreamMasterHub> hub
        )
    {
        _logger = logger;
        _hub = hub;
        _serviceProvider = serviceProvider;
        _broadcastTimer = new Timer(BroadcastMessage, null, 1000, 1000);
        _channeManager = new ChannelManager(logger, _serviceProvider);
    }

    public Setting setting
    {
        get
        {
            return FileUtil.GetSetting();
        }
    }

    public void Dispose()
    {
        _broadcastTimer?.Dispose();
        _channeManager.Dispose();
    }

    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        var allStatistics = _channeManager.GetAllStatisticsForAllUrls();

        return allStatistics;
    }

    public SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl)
    {
        _logger.LogInformation("Retrieving statistics for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        return _channeManager.GetSingleStreamStatisticsResult(streamUrl);
    }

    public async Task<Stream?> GetStream(ClientStreamerConfiguration config)
    {
        return await _channeManager.RegisterClient(config);
    }

    public void RemoveClient(ClientStreamerConfiguration config)
    {
        _channeManager.UnRegisterClient(config);
    }

    public void SimulateStreamFailure(string streamUrl)
    {
        //if (string.IsNullOrEmpty(streamUrl))
        //{
        //    _logger.LogWarning("Stream URL is empty or null, cannot simulate stream failure");
        //    return;
        //}

        //StreamInformation? _streamInformation = _streamManager.GetStreamInformationFromStreamUrl(streamUrl);

        //if (_streamInformation is not null)
        //{
        //    if (_streamInformation.StreamerCancellationToken is not null)
        //    {
        //        _streamInformation.StreamerCancellationToken.Cancel();
        //    }
        //    _logger.LogInformation("Simulating stream failure for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        //}
        //else
        //{
        //    _logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        //}
    }

    public void SimulateStreamFailureForAll()
    {
        //foreach (var s in _streamManager.GetStreamInformations())
        //{
        //    SimulateStreamFailure(s.StreamUrl);
        //}
    }

    private void BroadcastMessage(object? state)
    {
        if (BuildInfo.IsDebug)
        {
            var infos = _channeManager.GetStreamInformations().ToList();

            foreach (var info in infos.Where(a => a.RingBuffer != null))
            {
                _logger.LogInformation("{StreamUrl} {clientCount}", info.StreamUrl, info.ClientCount);
            }
        }

        _ = _hub.Clients.All.StreamStatisticsResultsUpdate(GetAllStatisticsForAllUrls());
    }
}
