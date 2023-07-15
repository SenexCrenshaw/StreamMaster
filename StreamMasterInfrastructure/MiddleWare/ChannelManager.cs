using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.MiddleWare;

public class ChannelManager : IDisposable, IChannelManager
{
    private readonly ConcurrentDictionary<int, ChannelStatus> _channelStatuses;
    private readonly ConcurrentDictionary<Guid, ClientStreamerConfiguration> _clients;

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;
    private readonly IStreamManager _streamManager;

    public ChannelManager(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _streamManager = new StreamManager(logger);
        _serviceProvider = serviceProvider;
        _channelStatuses = new();
        _clients = new();
    }

    private Setting setting => FileUtil.GetSetting();

    public static bool ShouldHandleFailover(ChannelStatus channelStatus)
    {
        StreamInformation _streamInformation = channelStatus.StreamInformation;
        return _streamInformation.ClientCount == 0 || _streamInformation.VideoStreamingCancellationToken.IsCancellationRequested || _streamInformation.StreamingTask.IsFaulted || _streamInformation.StreamingTask.IsCanceled;
    }

    public void Dispose()
    {
        _channelStatuses.Clear();
        _clients.Clear();
        GC.SuppressFinalize(this);
    }

    public List<StreamStatisticsResult> GetAllStatisticsForAllUrls()
    {
        List<StreamStatisticsResult> allStatistics = new();

        var infos = _streamManager.GetStreamInformations();
        foreach (var info in infos.Where(a => a.RingBuffer != null))
        {
            allStatistics.AddRange(info.RingBuffer.GetAllStatisticsForAllUrls());
        }

        return allStatistics;
    }

    public SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl)
    {
        return _streamManager.GetSingleStreamStatisticsResult(streamUrl);
    }

    public ICollection<StreamInformation> GetStreamInformations()
    {
        return _streamManager.GetStreamInformations();
    }

    public async Task<Stream> RegisterClient(ClientStreamerConfiguration config)
    {
        var channelStatus = await RegisterWithChannelManager(config);
        if (channelStatus is null || config.ReadBuffer is null)
        {
            throw new Exception("Could not register new channel manager");
        }
        return (Stream)config.ReadBuffer;
    }

    public void UnRegisterClient(ClientStreamerConfiguration config)
    {
        UnRegisterWithChannelManager(config);
    }

    private async Task ChannelWatcher(ChannelStatus channelStatus)
    {
        try
        {
            _logger.LogInformation("Channel watcher starting for channel: {videoStreamId}", channelStatus.VideoStreamId);

            bool quit = false;

            while (!channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
            {
                (quit, var newInfo) = await ProcessStreamStatus(channelStatus);
                if (quit)
                {
                    break;
                }

                await DelayWithCancellation(50, channelStatus.ChannelWatcherToken.Token);
            }
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Channel watcher error for channel: {videoStreamId}", channelStatus.VideoStreamId);
            return;
        }
        _logger.LogInformation("Channel watcher stopped for channel: {videoStreamId}", channelStatus.VideoStreamId);
        //_channelStatuses.TryRemove(channelStatus.VideoStreamId, out _);
    }

    private async Task DelayWithCancellation(int milliseconds, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(milliseconds, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task was cancelled");
            throw;
        }
    }

    private async Task<ChildVideoStreamDto?> GetNextChildVideoStream(ChannelStatus channelStatus)
    {
        var videoStreamId = channelStatus.VideoStreamId;

        using IServiceScope scope = _serviceProvider.CreateScope();
        IAppDbContext context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var result = await context.GetStreamsFromVideoStreamById(videoStreamId);
        if (result is null)
        {
            _logger.LogError("GetNextChildVideoStream could get videoStreams for id {VideoStreamId}", videoStreamId);
            return null;
        }

        var videoStreams = result.Value.childVideoStreamDtos;

        if (videoStreams is null || !videoStreams.Any() || videoStreams.Count == 0)
        {
            _logger.LogError("GetNextChildVideoStream could get videoStreams for id {VideoStreamId}", videoStreamId);
            return null;
        }

        var videoHandler = result.Value.videoStreamHandler == VideoStreamHandlers.SystemDefault ? VideoStreamHandlers.Loop : result.Value.videoStreamHandler;

        if (channelStatus.Rank == 0)
        {
            channelStatus.Rank = 1;
            return videoStreams[0];
        }

        if (channelStatus.Rank > videoStreams.Count)
        {
            if (videoHandler != VideoStreamHandlers.Loop)
                return null;

            channelStatus.Rank = 0;
        }

        return videoStreams.First(a => a.Rank == channelStatus.Rank++);
    }

    private async Task<StreamInformation?> HandleNextVideoStream(ChannelStatus channelStatus)
    {
        channelStatus.FailoverInProgress = true;

        DelayWithCancellation(200, channelStatus.ChannelWatcherToken.Token).Wait();

        var childVideoStreamDto = await GetNextChildVideoStream(channelStatus);

        if (childVideoStreamDto is null)
        {
            channelStatus.FailoverInProgress = false;
            return null;
        }

        channelStatus.StreamInformation = await _streamManager.GetOrCreateBuffer(childVideoStreamDto);

        if (channelStatus.StreamInformation is null)
        {
            channelStatus.FailoverInProgress = false;
            return null;
        }

        SwitchToNewStreamUrl(channelStatus);

        channelStatus.FailoverInProgress = false;
        return channelStatus.StreamInformation;
    }

    private async Task<(bool quit, StreamInformation? streamInformation)> ProcessStreamStatus(ChannelStatus channelStatus)
    {
        if (channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
        {
            return (true, null);
        }

        if (channelStatus.StreamInformation is null)
        {
            var ret = await HandleNextVideoStream(channelStatus);
            return (false, ret);
        }

        //if (!ShouldHandleFailover(channelStatus))
        //{
        //    return (false, null);
        //}

        //if (channelStatus.StreamInformation.FailoverInProgress)
        //{
        //    return (false, null);
        //}

        //if (channelStatus.StreamInformation.RingBuffer is null)
        //{
        //    var ret = HandleStreamAction(channelStatus);
        //    return (false, ret);
        //}

        // var st = _streamManager.GetStreamInformationFromStreamUrl(channelStatus.VideoStreamId);
        // if (channelStatus.Clients)
        //string? streamUrl = await GetUrl(channelStatus.VideoStreamId);
        //if (string.IsNullOrEmpty(streamUrl))
        //{
        //    return;
        //}

        //if (!ShouldHandleFailover(_streamInformation))
        //{
        //    return (false, null);
        //}

        //if (_streamInformation.FailoverInProgress)
        //{
        //    return (false, null);
        //}

        //if (_streamInformation.ClientCount == 0)
        //{
        //    StopStreamForNoClients(_streamInformation, streamUrl);
        //    return (true, null);
        //}

        //var ret = HandleStreamFailover(clientInfo, cancellationToken, _streamInformation);
        return (false, null);
    }

    private void RegisterClientsToNewStream(ICollection<Guid> clientIds, StreamInformation streamStreamInfo)
    {
        foreach (Guid clientId in clientIds)
        {
            RegisterClientToNewStream(clientId, streamStreamInfo);
        }
    }

    private void RegisterClientToNewStream(Guid clientId, StreamInformation streamStreamInfo)
    {
        var clientInfo = streamStreamInfo.GetStreamConfiguration(clientId);
        if (clientInfo is null)
        {
            _logger.LogError("Could not find client info for client id: {clientId}", clientId);
            return;
        }
        streamStreamInfo.RingBuffer.RegisterClient(clientId, clientInfo.ClientUserAgent);
        streamStreamInfo.SetClientBufferDelegate(clientId, () => streamStreamInfo.RingBuffer);
    }

    private async Task<ChannelStatus> RegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        ChannelStatus channelStatus;
        if (!_channelStatuses.ContainsKey(config.VideoStreamId))
        {
            channelStatus = new ChannelStatus(config.VideoStreamId);

            var info = await HandleNextVideoStream(channelStatus).ConfigureAwait(false);
            if (info is null || channelStatus.StreamInformation is null)
            {
                throw new ApplicationException("RegisterWithChannelManager HandleNextVideoStream should not be null");
            }

            _channelStatuses.TryAdd(config.VideoStreamId, channelStatus);
            _ = ChannelWatcher(channelStatus).ConfigureAwait(false);
        }
        else
        {
            channelStatus = _channelStatuses[config.VideoStreamId];
        }

        if (channelStatus.StreamInformation is null)
        {
            throw new ApplicationException("RegisterWithChannelManager StreamInformation is null");
        }

        _logger.LogInformation("ChannelManager added channel: {videoStreamId}", config.VideoStreamId);

        channelStatus.AddClientStreamerConfiguration(config);
        channelStatus.StreamInformation.RingBuffer.RegisterClient(config.ClientId, config.ClientUserAgent);
        config.ReadBuffer = new RingBufferReadStream(() => channelStatus.StreamInformation.RingBuffer, config);

        if (channelStatus is null)
        {
            throw new ApplicationException("GetUrl _channelStatus should be not have an entry");
        }

        return channelStatus;
    }

    private void SwitchToNewStreamUrl(ChannelStatus channelStatus)
    {
        if (channelStatus.StreamInformation is null)
        {
            return;
        }

        StreamInformation _streamInformation = channelStatus.StreamInformation;

        var clientIds = _streamInformation.RingBuffer.GetClientIds();

        RegisterClientsToNewStream(clientIds, _streamInformation);
    }

    private void UnRegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        if (!_channelStatuses.ContainsKey(config.VideoStreamId))
        {
            return;
        }

        var channelStatus = _channelStatuses[config.VideoStreamId];

        channelStatus.RemoveClientStreamerConfiguration(config.ClientId);

        if (channelStatus.GetClientStreamerCount() == 0)
        {
            if (channelStatus.StreamInformation is not null)
            {
                _streamManager.Stop(channelStatus.StreamInformation.StreamUrl);
            }

            channelStatus.ChannelWatcherToken.Cancel();
            _channelStatuses.TryRemove(config.VideoStreamId, out _);
            _logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId}", config.VideoStreamId);
        }
    }
}
