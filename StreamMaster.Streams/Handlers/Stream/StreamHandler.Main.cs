using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public sealed partial class StreamHandler : IStreamHandler
{
    public event EventHandler<StreamHandlerStopped> OnStreamingStoppedEvent;

    private readonly ILogger<WriteLogger> _writeLogger;
    private readonly ConcurrentDictionary<Guid, ClientStreamerConfiguration> clientStreamerConfigs = new();
    private readonly ILogger<IStreamHandler> logger;
    public SMStreamDto SMStream { get; }
    private int ProcessId { get; set; }
    public IOptionsMonitor<Setting> intSettings;
    private VideoInfo? _videoInfo = null;
    private CancellationTokenSource VideoStreamingCancellationToken { get; set; } = new();
    private readonly ILoggerFactory loggerFactory;
    public int ClientCount { get; set; } = 0;
    public bool IsFailed { get; set; }
    private int RestartCount { get; set; }
    public readonly StreamInfo StreamInfo;


    /// <summary>
    /// Initializes a new instance of the StreamHandler class, setting up the video stream handling,
    /// logging, and statistics tracking based on the provided parameters.
    /// </summary>
    /// <param name="videoStreamDto">A DTO containing the video stream information, such as stream URL, name, and ID.</param>
    /// <param name="processId">The process ID associated with this stream handler instance.</param>
    /// <param name="channelId">The unique identifier for the channel associated with the video stream.</param>
    /// <param name="channelName">The name of the channel associated with the video stream.</param>
    /// <param name="rank">The rank or priority of the video stream, used for sorting or prioritization.</param>
    /// <param name="memoryCache">An IMemoryCache instance for caching purposes within the stream handler.</param>
    /// <param name="loggerFactory">An ILoggerFactory instance used to create loggers for logging information and events.</param>
    /// <param name="inputStatisticsManager">An IChannelStreamingStatisticsManager instance for managing and tracking input statistics.</param>
    public StreamHandler(SMStreamDto SMStream, int processId, IOptionsMonitor<Setting> intSettings, ILoggerFactory loggerFactory, IStreamStreamingStatisticsManager streamStreamingStatisticsManager)
    {
        this.intSettings = intSettings;
        logger = loggerFactory.CreateLogger<StreamHandler>();
        this.loggerFactory = loggerFactory;
        this.SMStream = SMStream;
        ProcessId = processId;
        _writeLogger = loggerFactory.CreateLogger<WriteLogger>();

    }

    private void OnStreamingStopped(bool InputStreamError)
    {
        //streamStreamingStatisticsManager.UnRegisterStream(SMStream.Id);
        OnStreamingStoppedEvent?.Invoke(this, new StreamHandlerStopped { StreamUrl = SMStream.Url, InputStreamError = InputStreamError });
    }

    public void CancelStreamThread()
    {
        VideoStreamingCancellationToken?.Cancel();
    }

    public void Dispose()
    {
        //streamStreamingStatisticsManager.UnRegisterStream(SMStream.Id);
        clientStreamerConfigs.Clear();
        Stop();
        GC.SuppressFinalize(this);

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void Stop(bool inputStreamError = false)
    {
        SetFailed();

        if (VideoStreamingCancellationToken?.IsCancellationRequested == false)
        {
            VideoStreamingCancellationToken.Cancel();
        }

        if (ProcessId > 0)
        {

            try
            {
                _ = ProcessHelper.KillProcessById(ProcessId);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error killing process {ProcessId}.", ProcessId);
            }
        }


    }

    public void RegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration)
    {
        try
        {

            if (clientStreamerConfigs.TryAdd(streamerConfiguration.ClientId, streamerConfiguration))
            {
                //streamStatistics.IncrementClient();
                ++ClientCount;

                logger.LogInformation("RegisterClientStreamer for Client ID {ClientId} to Video Stream Id {videoStreamId} {name}", streamerConfiguration.ClientId, SMStream.Id, SMStream.Name);
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering stream configuration for client {ClientId} {name}", streamerConfiguration.ClientId, SMStream.Name);
        }
    }

    public void UnRegisterAllClientStreamers()
    {
        foreach (Guid key in clientStreamerConfigs.Keys)
        {
            _ = UnRegisterClientStreamer(key);
        }
    }

    public bool UnRegisterClientStreamer(Guid ClientId)
    {
        try
        {
            logger.LogInformation("UnRegisterClientStreamer ClientId: {ClientId} {name}", ClientId, SMStream.Name);
            bool result = clientStreamerConfigs.TryRemove(ClientId, out _);
            //streamStatistics.DecrementClient();
            --ClientCount;

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unregistering stream configuration for client {ClientId} {name}", ClientId, SMStream.Name);
            return false;
        }
    }

    public IEnumerable<ClientStreamerConfiguration> GetClientStreamerClientIdConfigs => clientStreamerConfigs.Values;


    public IEnumerable<Guid> GetClientStreamerClientIds()
    {
        return clientStreamerConfigs.Keys;
    }


    public void SetFailed()
    {
        IsFailed = true;
    }

    public bool HasClient(Guid clientId)
    {
        return clientStreamerConfigs.ContainsKey(clientId);
    }
}