using StreamMaster.Domain.Configuration;
using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public sealed partial class StreamHandler : IStreamHandler
{
    public event EventHandler<StreamHandlerStopped> OnStreamingStoppedEvent;

    private readonly ILogger<WriteLogger> _writeLogger;
    //private readonly IStatisticsManager statisticsManager;
    private readonly ConcurrentDictionary<Guid, ClientStreamerConfiguration> clientStreamerConfigs = new();
    private readonly ILogger<IStreamHandler> logger;

    private IStreamStreamingStatisticsManager streamStreamingStatisticsManager;
    private readonly StreamStreamingStatistic streamStatistics;

    private readonly IOptionsMonitor<Setting> intSettings;
    public SMChannelDto SMChannel { get; }
    public SMStreamDto SMStream { get; }
    public int ProcessId { get; set; }

    private VideoInfo? _videoInfo = null;
    private CancellationTokenSource VideoStreamingCancellationToken { get; set; } = new();
    private readonly ILoggerFactory loggerFactory;
    public int ClientCount { get; private set; } = 0;

    public bool IsFailed { get; private set; }
    public int RestartCount { get; set; }
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
    public StreamHandler(IChannelStatus channelStatus, int processId, IOptionsMonitor<Setting> intSettings, ILoggerFactory loggerFactory, IStreamStreamingStatisticsManager streamStreamingStatisticsManager)
    {
        logger = loggerFactory.CreateLogger<StreamHandler>();
        this.loggerFactory = loggerFactory;

        //BoundedChannelOptions options = new(1 * 1024)
        //{
        //    FullMode = BoundedChannelFullMode.DropWrite,
        //    SingleReader = true,
        //    SingleWriter = true
        //};

        //videoBuffer = Channel.CreateBounded<byte[]>(options);

        SMChannel = channelStatus.SMChannel;
        SMStream = channelStatus.SMStream;


        this.intSettings = intSettings;

        ProcessId = processId;

        this.streamStreamingStatisticsManager = streamStreamingStatisticsManager;

        _writeLogger = loggerFactory.CreateLogger<WriteLogger>();



        streamStatistics = streamStreamingStatisticsManager.RegisterStream(channelStatus.SMStream);

    }

    private void OnStreamingStopped(bool InputStreamError)
    {
        streamStreamingStatisticsManager.UnRegisterStream(SMStream.Id);
        OnStreamingStoppedEvent?.Invoke(this, new StreamHandlerStopped { StreamUrl = SMStream.Url, InputStreamError = InputStreamError });
    }



    public void Dispose()
    {
        streamStreamingStatisticsManager.UnRegisterStream(SMStream.Id);
        clientStreamerConfigs.Clear();
        Stop();
        GC.SuppressFinalize(this);

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void Stop()
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
                ProcessHelper.KillProcessById(ProcessId);

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
                streamStatistics.IncrementClient();
                ++ClientCount;

                logger.LogInformation("RegisterClientStreamer for Client ID {ClientId} to Video Stream Id {videoStreamId} {name}", streamerConfiguration.ClientId, SMStream.Id, SMStream.Name);
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering stream configuration for client {ClientId} {name}", streamerConfiguration.ClientId, SMStream.Name);
        }
    }


    public bool UnRegisterClientStreamer(Guid ClientId)
    {
        try
        {
            logger.LogInformation("UnRegisterClientStreamer ClientId: {ClientId} {name}", ClientId, SMStream.Name);
            bool result = clientStreamerConfigs.TryRemove(ClientId, out _);
            streamStatistics.DecrementClient();
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