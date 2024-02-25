using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMaster.Streams.Streams;


/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public sealed partial class StreamHandler : IStreamHandler
{
    public event EventHandler<StreamHandlerStopped> OnStreamingStoppedEvent;

    private readonly ILogger<WriteLogger> _writeLogger;

    private readonly ConcurrentDictionary<Guid, IClientStreamerConfiguration> clientStreamerConfigs = new();
    private readonly ILogger<IStreamHandler> logger;

    private readonly IInputStatisticsManager inputStatisticsManager;
    private readonly IInputStreamingStatistics inputStreamStatistics;
    private readonly Setting settings;

    public VideoStreamDto VideoStreamDto { get; }
    public int M3UFileId { get; }
    public int ProcessId { get; set; }
    public string StreamUrl { get; }
    public string VideoStreamId { get; }
    public string VideoStreamName { get; set; }

    private VideoInfo? _videoInfo = null;
    private CancellationTokenSource VideoStreamingCancellationToken { get; set; } = new();

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
    /// <param name="inputStatisticsManager">An IInputStatisticsManager instance for managing and tracking input statistics.</param>
    public StreamHandler(VideoStreamDto videoStreamDto, int processId, string channelId, string channelName, int rank, IOptionsMonitor<Setting> intsettings, ILoggerFactory loggerFactory, IInputStatisticsManager inputStatisticsManager)
    {
        settings = intsettings.CurrentValue;
        logger = loggerFactory.CreateLogger<StreamHandler>();
        VideoStreamDto = videoStreamDto;
        M3UFileId = videoStreamDto.M3UFileId;
        ProcessId = processId;
        StreamUrl = videoStreamDto.User_Url;
        VideoStreamId = videoStreamDto.Id;
        VideoStreamName = videoStreamDto.User_Tvg_name;
        this.inputStatisticsManager = inputStatisticsManager;

        _writeLogger = loggerFactory.CreateLogger<WriteLogger>();

        StreamInfo = new StreamInfo
        {
            ChannelId = channelId,
            ChannelName = channelName,
            VideoStreamId = videoStreamDto.Id,
            VideoStreamName = videoStreamDto.User_Tvg_name,
            Logo = videoStreamDto.User_Tvg_logo,
            StreamingProxyType = videoStreamDto.StreamingProxyType,
            StreamUrl = videoStreamDto.User_Url,

            Rank = rank
        };

        inputStreamStatistics = inputStatisticsManager.RegisterInputReader(StreamInfo);

    }

    private void OnStreamingStopped(bool InputStreamError)
    {
        OnStreamingStoppedEvent?.Invoke(this, new StreamHandlerStopped { StreamUrl = StreamUrl, InputStreamError = InputStreamError });
    }

    public void Dispose()
    {

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
                //string? procName = CheckProcessExists();
                //if (procName != null)
                //{
                //    Process process = Process.GetProcessById(ProcessId);
                //    process.Kill();
                //}
                KillProcess();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error killing process {ProcessId}.", ProcessId);
            }
        }


    }

    public void RegisterClientStreamer(IClientStreamerConfiguration streamerConfiguration)
    {
        try
        {

            _ = clientStreamerConfigs.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
            inputStreamStatistics.IncrementClient();
            ++ClientCount;

            logger.LogInformation("RegisterClientStreamer for Client ID {ClientId} to Video Stream Id {videoStreamId} {name}", streamerConfiguration.ClientId, VideoStreamId, VideoStreamName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering stream configuration for client {ClientId} {name}", streamerConfiguration.ClientId, VideoStreamName);
        }
    }


    public bool UnRegisterClientStreamer(Guid ClientId)
    {
        try
        {
            logger.LogInformation("UnRegisterClientStreamer ClientId: {ClientId} {name}", ClientId, VideoStreamName);
            bool result = clientStreamerConfigs.TryRemove(ClientId, out _);
            inputStreamStatistics.DecrementClient();
            --ClientCount;

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unregistering stream configuration for client {ClientId} {name}", ClientId, VideoStreamName);
            return false;
        }
    }

    private bool KillProcess()
    {
        try
        {
            Process process = Process.GetProcessById(ProcessId);
            return true;
        }
        catch (ArgumentException)
        {

        }
        return false;


    }

    //private string? CheckProcessExists()
    //{
    //    try
    //    {
    //        Process process = Process.GetProcessById(ProcessId);
    //        return process.ProcessName;
    //    }
    //    catch (ArgumentException)
    //    {
    //        return null;
    //    }
    //}


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