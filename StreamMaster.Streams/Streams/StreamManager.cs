using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public class StreamManager(IStreamHandlerFactory streamHandlerFactory, ILoggerFactory loggerFactory)
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

        if (streamHandler?.IsFailed == true)
        {
            _ = StopAndUnRegisterHandler(smStream.Url);
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

    private void StreamHandler_OnStreamingStoppedEvent(object? sender, StreamHandlerStopped StoppedEvent)
    {

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
   

    //public int UnRegisterClientStreamer(string url, string UniqueRequestId, string SMChannelName)
    //{
    //    //IStreamHandler? streamHandler = GetStreamHandlerFromStreamUrl(url);
    //    //if (streamHandler == null)
    //    //{
    //    //    return 0;
    //    //}

    //    //_ = streamHandler.UnRegisterChannel(UniqueRequestId);

    //    ////if (streamHandler.ChannelCount == 0)
    //    ////{
    //    ////    logger.LogInformation("No more clients, stopping streaming for {UniqueRequestId} {name}", UniqueRequestId, SMChannelName);
    //    ////    _ = StopAndUnRegisterHandler(url);
    //    ////}
    //    //return streamHandler.ChannelCount;
    //    return 0;
    //}
}