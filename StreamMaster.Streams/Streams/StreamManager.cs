using StreamMaster.Streams.Buffers;
using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public class StreamManager(IStreamHandlerFactory streamHandlerFactory, IClientStatisticsManager statisticsManager, ILoggerFactory loggerFactory)
    : IStreamManager
{
    public event EventHandler<StreamHandlerStopped> OnStreamingStoppedEvent;
    private readonly ConcurrentDictionary<string, IStreamHandler> _streamHandlers = new();
    private readonly object _disposeLock = new();
    private readonly ILogger<StreamManager> logger = loggerFactory.CreateLogger<StreamManager>();
    private bool _disposed = false;

    public bool IsHealthy()
    {
        return _streamHandlers.Values.All(handler => handler.IsHealthy());
    }

    public IDictionary<string, StreamHandlerMetrics> GetAggregatedMetrics()
    {
        Dictionary<string, StreamHandlerMetrics> metrics = [];

        foreach (KeyValuePair<string, IStreamHandler> kvp in _streamHandlers)
        {
            IStreamHandler handler = kvp.Value;
            metrics[kvp.Key] = new StreamHandlerMetrics
            {
                BytesRead = handler.GetBytesRead(),
                BytesWritten = handler.GetBytesWritten(),
                ClientCount = handler.GetClientCount(),
                Kbps = handler.GetKbps(),
                StartTime = handler.GetStartTime(),
                AverageLatency = handler.GetAverageLatency(),
                ErrorCount = handler.GetErrorCount()
            };
        }

        return metrics;
    }

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
        SMStreamDto smStream = channelStatus.SMStream;
        SMChannelDto smChannel = channelStatus.SMChannel;

        _ = _streamHandlers.TryGetValue(smStream.Url, out IStreamHandler? streamHandler);

        if (streamHandler is not null && streamHandler.IsFailed == true)
        {
            StopAndUnRegisterHandler(smStream.Url);
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
        //if (sender is not null and IStreamHandler streamHandler)
        //{
        //    //if (StoppedEvent.InputStreamError )
        //    //{
        //    //    OnStreamingStoppedEvent?.Invoke(sender, streamHandler);
        //    //}
        //    //else
        //    //{
        //    //    OnStreamingStoppedEvent?.Invoke(sender, streamHandler);
        //    //}
        //    //streamHandler.UnRegisterAllClientStreamers();
        //    //streamHandler.Stop();
        //    //StopAndUnRegisterHandler(streamHandler.SMStream.Url);
        //    OnStreamingStoppedEvent?.Invoke(sender, StoppedEvent);
        //}
        OnStreamingStoppedEvent?.Invoke(sender, StoppedEvent);
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

    //public async Task MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler, CancellationToken cancellationToken = default)
    //{
    //    List<ClientStreamerConfiguration> configs = oldStreamHandler.GetClientStreamerClientIdConfigs.ToList();

    //    if (!configs.Any())
    //    {
    //        return;
    //    }

    //    logger.LogInformation("Moving clients {count} from {OldStreamUrl} to {NewStreamUrl}", configs.Count(), oldStreamHandler.SMStream.Name, newStreamHandler.SMStream.Name);

    //    AddClientsToHandler(configs, newStreamHandler);

    //    foreach (ClientStreamerConfiguration? streamerConfiguration in configs)
    //    {

    //        if (streamerConfiguration == null)
    //        {
    //            logger.LogError("Error registering stream configuration for client {ClientId}, streamerConfiguration null.", streamerConfiguration.ClientId);
    //            return;
    //        }


    //        _ = oldStreamHandler.UnRegisterClientStreamer(streamerConfiguration.ClientId);

    //    }


    //    if (oldStreamHandler.ClientCount == 0)
    //    {
    //        //await Task.Delay(100);
    //        StopAndUnRegisterHandler(oldStreamHandler.SMStream.Url);

    //    }
    //}

    public void AddClientsToHandler(List<ClientStreamerConfiguration> clientIds, IStreamHandler streamHandler)
    {
        foreach (ClientStreamerConfiguration clientId in clientIds)
        {
            AddClientToHandler(clientId, streamHandler);
        }
    }

    public void AddClientToHandler(ClientStreamerConfiguration streamerConfiguration, IStreamHandler streamHandler)
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

    public bool StopAndUnRegisterHandler(string VideoStreamUrl)
    {
        if (_streamHandlers.TryRemove(VideoStreamUrl, out IStreamHandler? streamHandler))
        {
            streamHandler.Stop();
            return true;
        }

        logger.LogWarning("StopAndUnRegisterHandler {VideoStreamUrl} doesnt exist", VideoStreamUrl);
        return false;

    }

    public int UnRegisterClientStreamer(string url, Guid clientId, string SMChannelName)
    {
        IStreamHandler? streamHandler = GetStreamHandlerFromStreamUrl(url);
        if (streamHandler == null)
        {
            return 0;
        }

        streamHandler.UnRegisterClientStreamer(clientId);

        if (streamHandler.ClientCount == 0)
        {
            logger.LogInformation("No more clients, stopping streaming for {clientId} {name}", clientId, SMChannelName);
            StopAndUnRegisterHandler(url);
        }
        return streamHandler.ClientCount;
    }
}