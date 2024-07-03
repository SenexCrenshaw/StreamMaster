using StreamMaster.Streams.Buffers;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public class StreamManager(IStreamHandlerFactory streamHandlerFactory, IClientStatisticsManager statisticsManager, ILoggerFactory loggerFactory) : IStreamManager
{

    public event EventHandler<IStreamHandler> OnStreamingStoppedEvent;
    private readonly ConcurrentDictionary<string, IStreamHandler> _streamHandlers = new();
    private readonly object _disposeLock = new();
    private readonly ILogger<StreamManager> logger = loggerFactory.CreateLogger<StreamManager>();
    private bool _disposed = false;

    //public StreamManager(
    //    IStreamHandlerFactory streamHandlerFactory,
    //    ILogger<StreamManager> logger
    //)
    //{
    //    this.streamHandlerFactory = streamHandlerFactory;
    //    this.logger = logger;
    //}

    public void Dispose()
    {
        lock (_disposeLock)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                // Dispose of each stream controller
                foreach (IStreamHandler streamHandler in _streamHandlers.Values)
                {
                    try
                    {
                        streamHandler.Stop();
                        streamHandler.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error disposing stream handler {VideoStreamId}", streamHandler.SMStream.Id);
                    }
                }

                // Clear the dictionary
                _streamHandlers.Clear();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during disposing of StreamManager");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    public IStreamHandler? GetStreamHandler(string? smStreamId)
    {
        return string.IsNullOrEmpty(smStreamId)
            ? null
            : !_streamHandlers.TryGetValue(smStreamId, out IStreamHandler? streamHandler) ? null : streamHandler;
    }

    public async Task<IStreamHandler?> GetOrCreateStreamHandler(IChannelStatus channelStatus, CancellationToken cancellation = default)
    {
        SMStream smStream = channelStatus.SMStream;
        SMChannel smChannel = channelStatus.SMChannel;


        _ = _streamHandlers.TryGetValue(smStream.Url, out IStreamHandler? streamHandler);

        if (streamHandler is not null && streamHandler.IsFailed == true)
        {
            await StopAndUnRegisterHandler(smStream.Url);
            _ = _streamHandlers.TryGetValue(smStream.Url, out streamHandler);
        }

        if (streamHandler is null)
        {
            logger.LogInformation("Creating new handler for stream: {Id} {name}", smStream.Id, smStream.Name);
            streamHandler = await streamHandlerFactory.CreateStreamHandlerAsync(channelStatus, cancellation);

            if (streamHandler == null)
            {
                return null;
            }

            streamHandler.OnStreamingStoppedEvent += StreamHandler_OnStreamingStoppedEvent;
            bool test = _streamHandlers.TryAdd(smStream.Url, streamHandler);

            return streamHandler;
        }

        logger.LogInformation("Reusing handler for stream: {Id} {name}", smStream.Id, smStream.Name);
        return streamHandler;
    }
    //private async Task<IStreamHandler?> CreateStreamHandler(IChannelStatus channelStatus, CancellationToken cancellation)
    //{

    //    IStreamHandler? streamHandler = await streamHandlerFactory.CreateStreamHandlerAsync(channelStatus, cancellation);

    //    return streamHandler;
    //}

    private void StreamHandler_OnStreamingStoppedEvent(object? sender, StreamHandlerStopped StoppedEvent)
    {
        if (sender is IStreamHandler streamHandler)
        {
            if (streamHandler is not null)
            {
                if (StoppedEvent.InputStreamError && streamHandler.ClientCount > 0)
                {
                    OnStreamingStoppedEvent?.Invoke(sender, streamHandler);
                }
                else
                {
                    OnStreamingStoppedEvent?.Invoke(sender, streamHandler);
                }
            }
        }
    }

    public VideoInfo GetVideoInfo(string streamUrl)
    {
        IStreamHandler? handler = GetStreamHandlerFromStreamUrl(streamUrl);
        return handler is null ? new() : handler.GetVideoInfo();
    }

    public IStreamHandler? GetStreamHandlerFromStreamUrl(string streamUrl)
    {
        return _streamHandlers.Values.FirstOrDefault(x => x.SMStream.Url == streamUrl);
    }
    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamHandlers.Count(x => x.Value.SMStream.M3UFileId == m3uFileId);
    }

    public List<IStreamHandler> GetStreamHandlers()
    {
        return _streamHandlers.Values == null ? ([]) : ([.. _streamHandlers.Values]);
    }

    public async Task MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler, CancellationToken cancellationToken = default)
    {
        List<ClientStreamerConfiguration> configs = oldStreamHandler.GetClientStreamerClientIdConfigs.ToList();

        if (!configs.Any())
        {
            return;
        }

        logger.LogInformation("Moving clients {count} from {OldStreamUrl} to {NewStreamUrl}", configs.Count(), oldStreamHandler.SMStream.Name, newStreamHandler.SMStream.Name);

        await AddClientsToHandler(configs, newStreamHandler);

        foreach (ClientStreamerConfiguration? streamerConfiguration in configs)
        {

            if (streamerConfiguration == null)
            {
                logger.LogError("Error registering stream configuration for client {ClientId}, streamerConfiguration null.", streamerConfiguration.ClientId);
                return;
            }


            _ = oldStreamHandler.UnRegisterClientStreamer(streamerConfiguration.ClientId);

        }


        if (oldStreamHandler.ClientCount == 0)
        {
            //await Task.Delay(100);
            await StopAndUnRegisterHandler(oldStreamHandler.SMStream.Url);

        }
    }

    public async Task AddClientsToHandler(List<ClientStreamerConfiguration> clientIds, IStreamHandler streamHandler)
    {
        foreach (ClientStreamerConfiguration clientId in clientIds)
        {
            await AddClientToHandler(clientIds[0].SMChannel, clientId, streamHandler).ConfigureAwait(false);
        }
    }

    public async Task AddClientToHandler(SMChannel smChannel, ClientStreamerConfiguration streamerConfiguration, IStreamHandler streamHandler)
    {
        if (streamerConfiguration != null)
        {
            streamerConfiguration.ClientStream ??= new ClientReadStream(statisticsManager, loggerFactory, streamerConfiguration);

            logger.LogDebug("Adding client {ClientId} {ReaderID} ", streamerConfiguration.ClientId, streamerConfiguration.ClientStream?.Id ?? Guid.NewGuid());
            streamHandler.RegisterClientStreamer(streamerConfiguration);

        }
        else
        {
            logger.LogDebug("Error Add client, null");
        }
    }

    public IStreamHandler? GetStreamHandlerByClientId(Guid ClientId)
    {
        return _streamHandlers.Values.FirstOrDefault(x => x.HasClient(ClientId));
    }

    public async Task<bool> StopAndUnRegisterHandler(string VideoStreamUrl)
    {
        if (_streamHandlers.TryRemove(VideoStreamUrl, out IStreamHandler? streamHandler))
        {
            streamHandler.Stop();
            return true;
        }

        logger.LogWarning("StopAndUnRegisterHandler {VideoStreamUrl} doesnt exist", VideoStreamUrl);
        return false;

    }

}