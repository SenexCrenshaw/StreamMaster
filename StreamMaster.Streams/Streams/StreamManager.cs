using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public sealed class StreamManager(
    IClientStreamerManager clientStreamerManager,
    IStreamHandlerFactory streamHandlerFactory,
    ILogger<StreamManager> logger
    ) : IStreamManager
{

    public event EventHandler<IStreamHandler> OnStreamingStoppedEvent;
    private readonly ConcurrentDictionary<string, IStreamHandler> _streamHandlers = new();
    private readonly object _disposeLock = new();
    private bool _disposed = false;

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
        int rank = channelStatus.Rank;

        _ = _streamHandlers.TryGetValue(smStream.Url, out IStreamHandler? streamHandler);

        if (streamHandler is not null && streamHandler.IsFailed == true)
        {
            await StopAndUnRegisterHandler(smStream.Url);
            _ = _streamHandlers.TryGetValue(smStream.Url, out streamHandler);
        }

        if (streamHandler is null)
        {
            logger.LogInformation("Creating new handler for stream: {Id} {name}", smStream.Id, smStream.Name);
            streamHandler = await CreateStreamHandler(channelStatus, cancellation);
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
    private async Task<IStreamHandler?> CreateStreamHandler(IChannelStatus channelStatus, CancellationToken cancellation)
    {

        IStreamHandler? streamHandler = await streamHandlerFactory.CreateStreamHandlerAsync(channelStatus, cancellation);

        return streamHandler;
    }

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
        return _streamHandlers.Values.FirstOrDefault(x => x.StreamUrl == streamUrl);
    }
    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamHandlers.Count(x => x.Value.M3UFileId == m3uFileId);
    }

    public List<IStreamHandler> GetStreamHandlers()
    {
        return _streamHandlers.Values == null ? ([]) : ([.. _streamHandlers.Values]);
    }

    public async Task MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler, CancellationToken cancellationToken = default)
    {
        IEnumerable<Guid> ClientIds = oldStreamHandler.GetClientStreamerClientIds();

        if (!ClientIds.Any())
        {
            return;
        }

        logger.LogInformation("Moving clients {count} from {OldStreamUrl} to {NewStreamUrl}", ClientIds.Count(), oldStreamHandler.VideoStreamName, newStreamHandler.VideoStreamName);


        foreach (Guid clientId in ClientIds)
        {
            IClientStreamerConfiguration? streamerConfiguration = await clientStreamerManager.GetClientStreamerConfiguration(clientId, cancellationToken);

            if (streamerConfiguration == null)
            {
                logger.LogError("Error registering stream configuration for client {ClientId}, streamerConfiguration null.", clientId);
                return;
            }

            await clientStreamerManager.AddClientToHandler(streamerConfiguration.ClientId, newStreamHandler);
            _ = oldStreamHandler.UnRegisterClientStreamer(clientId);

        }


        if (oldStreamHandler.ClientCount == 0)
        {
            //await Task.Delay(100);
            await StopAndUnRegisterHandler(oldStreamHandler.StreamUrl);

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