﻿using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;

using System.Collections.Concurrent;
using System.Data;

namespace StreamMasterInfrastructure.MiddleWare;

public class ChannelManager : IDisposable, IChannelManager
{
    private readonly Timer _broadcastTimer;
    private readonly ConcurrentDictionary<string, ChannelStatus> _channelStatuses;
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hub;
    private readonly ILogger<ChannelManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStreamManager _streamManager;

    public ChannelManager(ILogger<ChannelManager> logger, IServiceProvider serviceProvider, IHubContext<StreamMasterHub, IStreamMasterHub> hub)
    {
        _logger = logger;
        _hub = hub;
        _streamManager = new StreamManager(logger);
        _broadcastTimer = new Timer(BroadcastMessage, null, 1000, 1000);
        _serviceProvider = serviceProvider;
        _channelStatuses = new();
    }

    private static Setting setting => FileUtil.GetSetting();

    public static bool ShouldHandleFailover(ChannelStatus channelStatus)
    {
        if (channelStatus.StreamInformation is null)
        {
            return false;
        }

        var _streamInformation = channelStatus.StreamInformation;
        return _streamInformation.ClientCount == 0 || _streamInformation.VideoStreamingCancellationToken.IsCancellationRequested || _streamInformation.StreamingTask.IsFaulted || _streamInformation.StreamingTask.IsCanceled;
    }

    public async Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId)
    {
        if (_channelStatuses.TryGetValue(playingVideoStreamId, out var channelStatus))
        {
            var oldInfo = channelStatus.StreamInformation;

            await HandleNextVideoStream(channelStatus, newVideoStreamId);

            if (oldInfo is not null && channelStatus.StreamInformation is not null)
            {
                var clientIds = channelStatus.ClientIds.Values.ToList();
                foreach (var client in clientIds)
                {
                    var c = channelStatus.StreamInformation.GetStreamConfiguration(client);

                    if (c == null)
                        continue;

                    oldInfo.UnRegisterStreamConfiguration(c);
                }
            }
            _logger.LogWarning("Channel changed for {videoStreamId} switching video stream id to {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);

            return;
        }
        _logger.LogWarning("Channel not found: {videoStreamId}", playingVideoStreamId);
        return;
    }

    public void Dispose()
    {
        _broadcastTimer?.Dispose();
        _channelStatuses.Clear();
        GC.SuppressFinalize(this);
    }

    public async Task FailClient(Guid clientId)
    {
        foreach (var channelStatus in _channelStatuses.Values.Where(a => a.StreamInformation is not null))
        {
            var c = channelStatus.StreamInformation.GetStreamConfiguration(clientId);

            if (c != null)
            {
                _logger.LogWarning("Failing client: {clientId}", clientId);
                c.CancellationTokenSource.Cancel();
                break;
            }
        }
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

    public async Task<Stream?> GetStream(ClientStreamerConfiguration config)
    {
        return await RegisterClient(config);
    }

    public void RemoveClient(ClientStreamerConfiguration config)
    {
        UnRegisterClient(config);
    }

    public void SimulateStreamFailure(string streamUrl)
    {
        var _streamInformation = _streamManager.GetStreamInformationFromStreamUrl(streamUrl);

        if (_streamInformation is not null)
        {
            if (_streamInformation.VideoStreamingCancellationToken is not null && !_streamInformation.VideoStreamingCancellationToken.IsCancellationRequested)
            {
                _streamInformation.VideoStreamingCancellationToken.Cancel();
            }
            _logger.LogInformation("Simulating stream failure for: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }
        else
        {
            _logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);
        }
    }

    public void SimulateStreamFailureForAll()
    {
        foreach (var s in _streamManager.GetStreamInformations())
        {
            s.VideoStreamingCancellationToken.Cancel();
        }
    }

    private void BroadcastMessage(object? state)
    {
        _ = _hub.Clients.All.StreamStatisticsResultsUpdate(GetAllStatisticsForAllUrls());
    }

    private async Task ChannelWatcher(ChannelStatus channelStatus)
    {
        try
        {
            _logger.LogInformation("Channel watcher starting for channel: {videoStreamId}", channelStatus.VideoStreamId);

            bool quit = false;

            while (!channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
            {
                quit = await ProcessStreamStatus(channelStatus);
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

    private async Task<ChildVideoStreamDto?> GetNextChildVideoStream(ChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IAppDbContext context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        IMapper mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        M3UFile? m3uFile;
        int allStreamsCount = 0;

        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            var newVideoStream = context.VideoStreams.AsNoTracking().ProjectTo<ChildVideoStreamDto>(mapper.ConfigurationProvider).FirstOrDefault(a => a.Id == overrideNextVideoStreamId);
            if (newVideoStream == null)
            {
                _logger.LogError("GetNextChildVideoStream could not get videoStream for id {VideoStreamId}", overrideNextVideoStreamId);
                return null;
            }

            m3uFile = await context.M3UFiles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == newVideoStream.M3UFileId);
            if (m3uFile == null)
            {
                return null;
            }

            allStreamsCount = _streamManager.GetStreamsCountForM3UFile(newVideoStream.M3UFileId);

            if (newVideoStream.Id != channelStatus.VideoStreamId && allStreamsCount >= m3uFile.MaxStreamCount)
            {
                _logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", newVideoStream.MaxStreams, setting.CleanURLs ? "url removed" : newVideoStream.User_Url);
            }
            else
            {
                return newVideoStream;
            }
        }

        var result = await context.GetStreamsFromVideoStreamById(channelStatus.VideoStreamId);
        if (result == null)
        {
            _logger.LogError("GetNextChildVideoStream could not get videoStream for id {VideoStreamId}", channelStatus.VideoStreamId);
            return null;
        }

        var videoStreams = result.Value.childVideoStreamDtos.OrderBy(a => a.Rank).ToArray();
        if (!videoStreams.Any())
        {
            _logger.LogError("GetNextChildVideoStream could not get child videoStreams for id {VideoStreamId}", channelStatus.VideoStreamId);
            return null;
        }

        var videoHandler = result.Value.videoStreamHandler == VideoStreamHandlers.SystemDefault ? VideoStreamHandlers.Loop : result.Value.videoStreamHandler;

        if (channelStatus.Rank >= videoStreams.Length)
        {
            //if (videoHandler != VideoStreamHandlers.Loop)
            //    return null;

            channelStatus.Rank = 0;
        }

        while (channelStatus.Rank < videoStreams.Length)
        {
            var toReturn = videoStreams[channelStatus.Rank++];

            m3uFile = await context.M3UFiles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == toReturn.M3UFileId);
            if (m3uFile == null)
            {
                return null;
            }

            allStreamsCount = _streamManager.GetStreamsCountForM3UFile(toReturn.M3UFileId);
            if (allStreamsCount >= m3uFile.MaxStreamCount)
            {
                _logger.LogInformation("Max stream count {MaxStreams} reached for stream: {StreamUrl}", toReturn.MaxStreams, setting.CleanURLs ? "url removed" : toReturn.User_Url);
                continue;
            }

            return toReturn;
        }

        return null;
    }

    private ICollection<IStreamInformation> GetStreamInformations()
    {
        return _streamManager.GetStreamInformations();
    }

    private async Task<bool> HandleNextVideoStream(ChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        channelStatus.FailoverInProgress = true;

        DelayWithCancellation(200, channelStatus.ChannelWatcherToken.Token).Wait();

        var childVideoStreamDto = await GetNextChildVideoStream(channelStatus, overrideNextVideoStreamId);
        if (childVideoStreamDto is null)
        {
            channelStatus.FailoverInProgress = false;
            return false;
        }

        ICollection<ClientStreamerConfiguration>? oldConfigs = null;
        if (channelStatus.StreamInformation is not null)
        {
            oldConfigs = channelStatus.StreamInformation.GetStreamConfigurations();
        }

        channelStatus.StreamInformation = await _streamManager.GetOrCreateBuffer(childVideoStreamDto, channelStatus.VideoStreamId, channelStatus.VideoStreamName, channelStatus.Rank);

        if (channelStatus.StreamInformation is null)
        {
            channelStatus.FailoverInProgress = false;
            return false;
        }

        if (oldConfigs is not null)
        {
            RegisterClientsToNewStream(oldConfigs, channelStatus.StreamInformation);
        }

        channelStatus.FailoverInProgress = false;
        return true;
    }

    private async Task<bool> ProcessStreamStatus(ChannelStatus channelStatus)
    {
        if (channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
        {
            return true;
        }

        if (channelStatus.StreamInformation is null)
        {
            var handled = await HandleNextVideoStream(channelStatus);
            return !handled;
        }

        if (channelStatus.StreamInformation.VideoStreamingCancellationToken.IsCancellationRequested)
        {
            _streamManager.Stop(channelStatus.StreamInformation.StreamUrl);

            var handled = await HandleNextVideoStream(channelStatus);
            return !handled;
        }

        return false;
    }

    private async Task<Stream?> RegisterClient(ClientStreamerConfiguration config)
    {
        var channelStatus = await RegisterWithChannelManager(config);
        if (channelStatus is null || config.ReadBuffer is null)
        {
            return null;
        }
        return (Stream)config.ReadBuffer;
    }

    private void RegisterClientsToNewStream(ICollection<ClientStreamerConfiguration> configs, IStreamInformation streamStreamInfo)
    {
        foreach (var config in configs)
        {
            RegisterClientToNewStream(config, streamStreamInfo);
        }
    }

    private void RegisterClientToNewStream(ClientStreamerConfiguration config, IStreamInformation streamStreamInfo)
    {
        _logger.LogInformation("Registered client id: {clientId} to videostream url {StreamUrl}", config.ClientId, setting.CleanURLs ? "url removed" : streamStreamInfo.StreamUrl);

        streamStreamInfo.RegisterStreamConfiguration(config);
    }

    private async Task<ChannelStatus?> RegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        ChannelStatus channelStatus;
        if (!_channelStatuses.ContainsKey(config.VideoStreamId))
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IAppDbContext context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var videoStream = await context.VideoStreams.AsNoTracking().FirstOrDefaultAsync(a => a.Id == config.VideoStreamId);

            if (videoStream is null)
                return null;

            config.VideoStreamName = videoStream.User_Tvg_name;
            channelStatus = new ChannelStatus(config.VideoStreamId, config.VideoStreamName);

            if (!await HandleNextVideoStream(channelStatus).ConfigureAwait(false) || channelStatus.StreamInformation is null)
            {
                return null;
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
            return null;
        }

        channelStatus.ClientIds.TryAdd(config.ClientId, config.ClientId);

        _logger.LogInformation("ChannelManager added channel: {videoStreamId}", config.VideoStreamId);

        config.ReadBuffer = new RingBufferReadStream(() => channelStatus.StreamInformation.RingBuffer, config);

        channelStatus.StreamInformation.RegisterStreamConfiguration(config);

        if (channelStatus is null)
        {
            return null;
        }

        return channelStatus;
    }

    private void UnRegisterClient(ClientStreamerConfiguration config)
    {
        UnRegisterWithChannelManager(config);
    }

    private void UnRegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        if (!_channelStatuses.ContainsKey(config.VideoStreamId))
        {
            return;
        }

        var channelStatus = _channelStatuses[config.VideoStreamId];

        if (channelStatus.StreamInformation is not null)
        {
            channelStatus.StreamInformation.UnRegisterStreamConfiguration(config);

            if (channelStatus.StreamInformation.ClientCount == 0)
            {
                channelStatus.ChannelWatcherToken.Cancel();

                if (channelStatus.StreamInformation is not null)
                {
                    _streamManager.Stop(channelStatus.StreamInformation.StreamUrl);
                }

                _channelStatuses.TryRemove(config.VideoStreamId, out _);
                _logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId}", config.VideoStreamId);
            }
        }

        channelStatus.ClientIds.TryRemove(config.ClientId, out _);
    }
}
