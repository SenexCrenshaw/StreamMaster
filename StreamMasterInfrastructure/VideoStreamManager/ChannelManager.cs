using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Extensions;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ChannelManager(ILogger<ChannelManager> logger, IStreamSwitcher streamSwitcher, IChannelService channelService, IStreamManager streamManager, IServiceProvider serviceProvider) : IChannelManager
{
    public async Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId)
    {
        logger.LogDebug("Starting ChangeVideoStreamChannel with playingVideoStreamId: {playingVideoStreamId} and newVideoStreamId: {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);

        IChannelStatus? channelStatus = channelService.GetChannelStatus(playingVideoStreamId);

        if (channelStatus != null)
        {
            logger.LogDebug("Channel status found for playingVideoStreamId: {playingVideoStreamId}", playingVideoStreamId);

            IStreamHandler? oldInfo = channelStatus.StreamHandler;

            if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus, newVideoStreamId))
            {
                logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newVideoStreamId}", newVideoStreamId);
                return;
            }

            if (oldInfo is not null && channelStatus.StreamHandler is not null)
            {
                foreach (Guid clientID in channelStatus.GetChannelClientIds)
                {
                    ClientStreamerConfiguration? c = channelStatus.StreamHandler.GetClientStreamerConfiguration(clientID);

                    if (c == null)
                    {
                        logger.LogDebug("Stream configuration is null for client: {clientID}, skipping unregistration", clientID);
                        continue;
                    }

                    _ = oldInfo.UnRegisterClientStreamer(c);
                    logger.LogDebug("Unregistered stream configuration for client: {clientID}", clientID);
                }
            }
            logger.LogWarning("Channel changed for {videoStreamId} switching video stream id to {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);
            logger.LogDebug("Exiting ChangeVideoStreamChannel after channel change");

            return;
        }
        logger.LogWarning("Channel not found: {videoStreamId}", playingVideoStreamId);
        logger.LogDebug("Exiting ChangeVideoStreamChannel due to channel not found");

        return;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void FailClient(Guid clientId)
    {
        logger.LogDebug("Starting FailClient with clientId: {clientId}", clientId);

        foreach (IStreamHandler streamHandler in channelService.GetStreamHandlers())
        {
            ClientStreamerConfiguration? c = streamHandler.GetClientStreamerConfiguration(clientId);

            if (c != null)
            {
                c.ClientMasterToken.Cancel();
                logger.LogWarning("Failing client: {clientId}", clientId);
                logger.LogDebug("Cancelled CancellationTokenSource for client: {clientId}", clientId);
                logger.LogDebug("Exiting FailClient after failing client: {clientId}", clientId);
                break;
            }
        }
        logger.LogDebug("Exiting FailClient, no matching client found for clientId: {clientId}", clientId);
    }

    public async Task<Stream?> GetStream(ClientStreamerConfiguration config)
    {
        return await RegisterClient(config);
    }

    public void RemoveClient(ClientStreamerConfiguration config)
    {
        UnRegisterWithChannelManager(config);
    }

    public static bool ShouldHandleFailover(ChannelStatus channelStatus)
    {
        if (channelStatus.StreamHandler is null)
        {
            return false;
        }

        IStreamHandler _streamInformation = channelStatus.StreamHandler;
        return _streamInformation.ClientCount == 0 || _streamInformation.VideoStreamingCancellationToken.IsCancellationRequested;
    }

    public void SimulateStreamFailure(string streamUrl)
    {
        IStreamHandler? _streamInformation = streamManager.GetStreamInformationFromStreamUrl(streamUrl);

        if (_streamInformation is not null)
        {
            if (_streamInformation.VideoStreamingCancellationToken?.IsCancellationRequested == false)
            {
                _streamInformation.VideoStreamingCancellationToken.Cancel();
            }
            logger.LogInformation("Simulating stream failure for: {StreamUrl}", streamUrl);
        }
        else
        {
            logger.LogWarning("Stream not found, cannot simulate stream failure: {StreamUrl}", streamUrl);
        }
    }

    public void SimulateStreamFailureForAll()
    {
        foreach (IStreamHandler s in streamManager.GetStreamInformations())
        {
            s.VideoStreamingCancellationToken.Cancel();
        }
    }

    private async Task ChannelWatcher(IChannelStatus channelStatus)
    {
        try
        {
            logger.LogInformation("Channel watcher starting for channel: {videoStreamId}", channelStatus.VideoStreamId);

            bool quit = false;

            while (!channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
            {
                quit = await ProcessStreamStatus(channelStatus);
                if (quit)
                {
                    logger.LogDebug("Exiting ChannelWatcher loop due to quit condition for channel: {VideoStreamId}", channelStatus.VideoStreamId);
                    break;
                }

                await channelStatus.ChannelWatcherToken.Token.ApplyDelay(50);
            }
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Channel watcher error for channel: {videoStreamId}", channelStatus.VideoStreamId);
            logger.LogDebug("Exiting ChannelWatcher due to error for channel: {VideoStreamId}", channelStatus.VideoStreamId);
            return;
        }

        logger.LogInformation("Channel watcher stopped for channel: {videoStreamId}", channelStatus.VideoStreamId);
        logger.LogDebug("Exiting ChannelWatcher for channel: {VideoStreamId}", channelStatus.VideoStreamId);
    }

    private async Task<bool> ProcessStreamStatus(IChannelStatus channelStatus)
    {
        if (channelStatus.ChannelWatcherToken.Token.IsCancellationRequested)
        {
            logger.LogDebug("ChannelWatcherToken cancellation requested for channelStatus: {channelStatus}", channelStatus);
            logger.LogDebug("Exiting ProcessStreamStatus with true due to ChannelWatcherToken cancellation request");
            return true;
        }

        if (channelStatus.StreamHandler is null)
        {
            logger.LogDebug("ChannelStatus StreamInformation is null for channelStatus: {channelStatus}, attempting to handle next video stream", channelStatus);
            bool handled;
            try
            {
                handled = await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus);
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Task was cancelled");
                throw;
            }

            logger.LogDebug("Exiting ProcessStreamStatus with {handled} after handling next video stream", !handled);
            return !handled;
        }

        if (channelStatus.StreamHandler.VideoStreamingCancellationToken.IsCancellationRequested)
        {
            logger.LogDebug("VideoStreamingCancellationToken cancellation requested for channelStatus: {channelStatus}, stopping stream and attempting to handle next video stream", channelStatus);
            _ = streamManager.Stop(channelStatus.StreamHandler.StreamUrl);
            try
            {
                bool handled = await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus);
                logger.LogDebug("Exiting ProcessStreamStatus with {handled} after stopping streaming and handling next video stream", !handled);
                return !handled;
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Task was cancelled");
                throw;
            }
        }

        return false;
    }

    private async Task<Stream?> RegisterClient(ClientStreamerConfiguration config)
    {
        logger.LogDebug("Starting RegisterClient with config: {config}", config.ClientId);

        IChannelStatus? channelStatus = await RegisterWithChannelManager(config);
        if (channelStatus is null || config.ReadBuffer is null)
        {
            logger.LogDebug("Exiting RegisterClient with null due to channelStatus or config.ReadBuffer being null");
            return null;
        }

        logger.LogDebug("Finished RegisterClient, returning config.ReadBuffer");
        return (Stream)config.ReadBuffer;
    }

    private async Task<IChannelStatus?> RegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        logger.LogDebug("Starting RegisterWithChannelManager with config: {config}", config.ClientId);

        IChannelStatus? channelStatus;
        if (!channelService.HasChannel(config.VideoStreamId))
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            VideoStreamDto? videoStream = await repository.VideoStream.GetVideoStreamById(config.VideoStreamId);

            if (videoStream is null)
            {
                return null;
            }

            config.VideoStreamName = videoStream.User_Tvg_name;
            channelStatus = new ChannelStatus(config.VideoStreamId, config.VideoStreamName);

            try
            {
                if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus).ConfigureAwait(false) || channelStatus.StreamHandler is null)
                {
                    return null;
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Task was cancelled");
                throw;
            }

            channelService.RegisterChannel(config.VideoStreamId, config.VideoStreamName);
            _ = ChannelWatcher(channelStatus).ConfigureAwait(false);
        }
        else
        {
            channelStatus = channelService.GetChannelStatus(config.VideoStreamId);
        }

        if (channelStatus is null)
        {
            logger.LogDebug("Exiting RegisterWithChannelManager with null due to channelStatus being null");
            return null;
        }

        if (channelStatus.StreamHandler is null)
        {
            logger.LogError("Failed to get or create buffer for childVideoStreamDto in RegisterWithChannelManager");
            logger.LogDebug("Exiting RegisterWithChannelManager with null due to failure to get or create buffer");
            return null;
        }

        channelStatus.AddToClientIds(config.ClientId);

        logger.LogInformation("ChannelManager added channel: {videoStreamId}", config.VideoStreamId);

        config.ReadBuffer = new RingBufferReadStream(() => channelStatus.StreamHandler.RingBuffer, config);

        channelStatus.StreamHandler.RegisterClientStreamer(config);

        logger.LogDebug("Finished RegisterWithChannelManager with config: {config}", config.ClientId);

        return channelStatus;
    }

    private void UnRegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        logger.LogDebug("Starting UnRegisterWithChannelManager with config: {config}", config.ClientId);

        if (!channelService.HasChannel(config.VideoStreamId))
        {
            logger.LogDebug("Exiting UnRegisterWithChannelManager due to VideoStreamId not found in _channelStatuses");
            return;
        }

        IChannelStatus? channelStatus = channelService.GetChannelStatus(config.VideoStreamId);

        if (channelStatus is not null)
        {
            if (channelStatus.StreamHandler is not null)
            {
                _ = channelStatus.StreamHandler.UnRegisterClientStreamer(config);

                if (channelStatus.StreamHandler.ClientCount == 0)
                {
                    channelStatus.ChannelWatcherToken.Cancel();

                    if (channelStatus.StreamHandler is not null)
                    {
                        _ = streamManager.Stop(channelStatus.StreamHandler.StreamUrl);
                    }
                    channelService.UnregisterChannel(config.VideoStreamId);
                    logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId}", config.VideoStreamId);
                    logger.LogDebug("Exiting UnRegisterWithChannelManager after stopping streaming due to no more clients");
                }
            }

            channelStatus.RemoveClientId(config.ClientId);
        }
        logger.LogDebug("Finished UnRegisterWithChannelManager with config: {config}", config.ClientId);
    }
}