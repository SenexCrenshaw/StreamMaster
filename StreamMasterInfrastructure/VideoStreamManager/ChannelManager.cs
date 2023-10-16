using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Extensions;
using StreamMasterDomain.Repository;

using StreamMasterInfrastructure.VideoStreamManager.Buffers;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ChannelManager(ILogger<ChannelManager> logger, IBroadcastService broadcastService, IStreamSwitcher streamSwitcher, IChannelService channelService, IStreamManager streamManager, IServiceProvider serviceProvider) : IChannelManager
{
    public async Task ChangeVideoStreamChannel(string playingVideoStreamId, string newVideoStreamId)
    {
        logger.LogDebug("Starting ChangeVideoStreamChannel with playingVideoStreamId: {playingVideoStreamId} and newVideoStreamId: {newVideoStreamId}", playingVideoStreamId, newVideoStreamId);

        IChannelStatus? channelStatus = channelService.GetChannelStatus(playingVideoStreamId);

        if (channelStatus != null)
        {
            logger.LogDebug("Channel status found for playingVideoStreamId: {playingVideoStreamId}", playingVideoStreamId);

            IStreamHandler? oldStreamHandler = streamManager.GetStreamHandler(newVideoStreamId).DeepCopy();

            //IStreamHandler? oldInfo = channelStatus.StreamHandler;

            if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus, newVideoStreamId))
            {
                logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newVideoStreamId}", newVideoStreamId);
                return;
            }

            IStreamHandler? newStreamHandler = streamManager.GetStreamHandler(newVideoStreamId).DeepCopy();

            if (oldStreamHandler is not null && newStreamHandler is not null)
            {
                foreach (Guid clientID in channelStatus.GetChannelClientIds)
                {
                    ClientStreamerConfiguration? c = newStreamHandler.GetClientStreamerConfiguration(clientID);

                    if (c == null)
                    {
                        logger.LogDebug("Stream configuration is null for client: {clientID}, skipping unregistration", clientID);
                        continue;
                    }

                    _ = oldStreamHandler.UnRegisterClientStreamer(c);
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
        broadcastService.StopBroadcasting();
        GC.SuppressFinalize(this);

    }

    public void FailClient(Guid clientId)
    {
        logger.LogDebug("Starting FailClient with clientId: {clientId}", clientId);

        foreach (IStreamHandler streamHandler in streamManager.GetStreamHandlers())
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
        broadcastService.StartBroadcasting();
        return await RegisterClient(config);
    }

    public void RemoveClient(ClientStreamerConfiguration config)
    {
        UnRegisterWithChannelManager(config);
    }

    public bool ShouldHandleFailover(ChannelStatus channelStatus)
    {
        IStreamHandler? streamHandler = streamManager.GetStreamHandler(channelStatus.VideoStreamId);

        if (streamHandler is null)
        {
            return false;
        }

        return streamHandler.ClientCount == 0 || streamHandler.VideoStreamingCancellationToken.IsCancellationRequested;
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

        IStreamHandler? streamHandler = streamManager.GetStreamHandler(channelStatus.VideoStreamId);
        if (streamHandler is null)
        {
            logger.LogDebug("ChannelStatus StreamInformation is null for channelStatus: {channelStatus}, attempting to switch to next video stream", channelStatus);
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

            logger.LogDebug("Exiting ProcessStreamStatus with {handled} after switching to next video stream", !handled);
            return !handled;
        }

        if (streamHandler.VideoStreamingCancellationToken.IsCancellationRequested)
        {
            logger.LogDebug("VideoStreamingCancellationToken cancellation requested for channelStatus: {channelStatus}, stopping stream and attempting to handle next video stream", channelStatus);
            _ = streamManager.Stop(streamHandler.StreamUrl);
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

        IStreamHandler? streamHandler = streamManager.GetStreamHandler(config.VideoStreamId);
        IChannelStatus? channelStatus;

        if (!channelService.HasChannel(config.VideoStreamId))
        {
            channelStatus = await RegisterNewChannel(config, streamHandler);
        }
        else
        {
            channelStatus = HandleExistingChannel(config, streamHandler);
        }

        if (channelStatus == null || streamHandler == null)
        {
            logger.LogError("Failed to register with channel manager.");
            channelService.UnregisterChannel(config.VideoStreamId);
            return null;
        }

        CompleteClientRegistration(channelStatus, streamHandler, config);

        logger.LogDebug("Finished RegisterWithChannelManager with config: {config}", config.ClientId);
        return channelStatus;
    }

    private async Task<IChannelStatus?> RegisterNewChannel(ClientStreamerConfiguration config, IStreamHandler? streamHandler)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        VideoStreamDto? videoStream = await repository.VideoStream.GetVideoStreamById(config.VideoStreamId);

        if (videoStream == null)
        {
            return null;
        }

        config.VideoStreamName = videoStream.User_Tvg_name;
        IChannelStatus channelStatus = channelService.RegisterChannel(config.VideoStreamId, config.VideoStreamName);

        if (streamHandler == null)
        {
            if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus).ConfigureAwait(false))
            {
                return null;
            }
            streamHandler = streamManager.GetStreamHandler(config.VideoStreamId);
        }

        if (streamHandler == null)
        {
            logger.LogError("Failed to get streamHandler for videostream with id {id}", config.VideoStreamId);
            return null;
        }

        _ = ChannelWatcher(channelStatus).ConfigureAwait(false);
        return channelStatus;
    }

    private IChannelStatus? HandleExistingChannel(ClientStreamerConfiguration config, IStreamHandler? streamHandler)
    {
        IChannelStatus? channelStatus = channelService.GetChannelStatus(config.VideoStreamId);
        if (streamHandler == null)
        {
            logger.LogError("Failed to get streamHandler for videostream with id {id}", config.VideoStreamId);
            return null;
        }
        return channelStatus;
    }

    private void CompleteClientRegistration(IChannelStatus channelStatus, IStreamHandler streamHandler, ClientStreamerConfiguration config)
    {
        channelStatus.AddToClientIds(config.ClientId);
        logger.LogInformation("ChannelManager added channel: {videoStreamId}", config.VideoStreamId);
        config.ReadBuffer = new RingBufferReadStream(() => streamHandler.RingBuffer, config);
        streamHandler.RegisterClientStreamer(config);
    }



    //private async Task<IChannelStatus?> RegisterWithChannelManager(ClientStreamerConfiguration config)
    //{
    //    logger.LogDebug("Starting RegisterWithChannelManager with config: {config}", config.ClientId);

    //    IChannelStatus? channelStatus;
    //    IStreamHandler? streamHandler = streamManager.GetStreamHandler(config.VideoStreamId);

    //    if (!channelService.HasChannel(config.VideoStreamId))
    //    {
    //        using IServiceScope scope = serviceProvider.CreateScope();
    //        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
    //        VideoStreamDto? videoStream = await repository.VideoStream.GetVideoStreamById(config.VideoStreamId);

    //        if (videoStream is null)
    //        {
    //            return null;
    //        }

    //        config.VideoStreamName = videoStream.User_Tvg_name;

    //        channelStatus = channelService.RegisterChannel(config.VideoStreamId, config.VideoStreamName);

    //        if (streamHandler is null)
    //        {
    //            if (!await streamSwitcher.SwitchToNextVideoStreamAsync(channelStatus).ConfigureAwait(false) || streamHandler is null)
    //            {
    //                channelService.UnregisterChannel(config.VideoStreamId);
    //                return null;
    //            }
    //            streamHandler = streamManager.GetStreamHandler(config.VideoStreamId);
    //            if (streamHandler is null)
    //            {
    //                logger.LogError("Failed to get streamHandler for videostream with id {id}", config.VideoStreamId);
    //                channelService.UnregisterChannel(config.VideoStreamId);
    //                return null;
    //            }
    //        }

    //        _ = ChannelWatcher(channelStatus).ConfigureAwait(false);
    //    }
    //    else
    //    {
    //        channelStatus = channelService.GetChannelStatus(config.VideoStreamId);
    //    }


    //    if (channelStatus is null)
    //    {
    //        logger.LogError("Exiting RegisterWithChannelManager with null due to channelStatus being null");
    //        channelService.UnregisterChannel(config.VideoStreamId);
    //        return null;
    //    }

    //    if (streamHandler is null)
    //    {
    //        logger.LogError("Failed to get streamHandler for videostream with id {id}", config.VideoStreamId);
    //        channelService.UnregisterChannel(config.VideoStreamId);
    //        return null;
    //    }

    //    channelStatus.AddToClientIds(config.ClientId);

    //    logger.LogInformation("ChannelManager added channel: {videoStreamId}", config.VideoStreamId);

    //    config.ReadBuffer = new RingBufferReadStream(() => streamHandler.RingBuffer, config);

    //    streamHandler.RegisterClientStreamer(config);

    //    logger.LogDebug("Finished RegisterWithChannelManager with config: {config}", config.ClientId);

    //    return channelStatus;
    //}

    private void UnRegisterWithChannelManager(ClientStreamerConfiguration config)
    {
        logger.LogDebug("Starting UnRegisterWithChannelManager with config: {config}", config.ClientId);

        if (!channelService.HasChannel(config.VideoStreamId))
        {
            logger.LogDebug("Exiting UnRegisterWithChannelManager due to VideoStreamId not found in _channelStatuses");
            return;
        }

        IChannelStatus? channelStatus = channelService.GetChannelStatus(config.VideoStreamId);
        IStreamHandler? StreamHandler = streamManager.GetStreamHandler(config.VideoStreamId);

        if (StreamHandler is not null)
        {
            _ = StreamHandler.UnRegisterClientStreamer(config);

            if (StreamHandler.ClientCount == 0)
            {
                channelStatus.ChannelWatcherToken.Cancel();

                if (StreamHandler is not null)
                {
                    _ = streamManager.Stop(config.VideoStreamId);
                }
                channelService.UnregisterChannel(config.VideoStreamId);
                logger.LogInformation("ChannelManager No more clients, stopping streaming for {videoStreamId}", config.VideoStreamId);
                logger.LogDebug("Exiting UnRegisterWithChannelManager after stopping streaming due to no more clients");
            }
        }

        channelStatus?.RemoveClientId(config.ClientId);
        logger.LogDebug("Finished UnRegisterWithChannelManager with config: {config}", config.ClientId);
    }
}