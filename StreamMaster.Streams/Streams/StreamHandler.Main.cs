using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Models;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Streams;


/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public sealed partial class StreamHandler : IStreamHandler
{
    public event EventHandler<StreamHandlerStopped> OnStreamingStoppedEvent;

    private readonly ILogger<WriteLogger> _writeLogger;

    private readonly ConcurrentDictionary<Guid, IClientStreamerConfiguration> clientStreamerConfigs = new();
    private readonly IMemoryCache memoryCache;
    private readonly ILogger<IStreamHandler> logger;

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

    public StreamHandler(VideoStreamDto videoStreamDto, int processId, IMemoryCache memoryCache, ILoggerFactory loggerFactory)
    {
        this.memoryCache = memoryCache;
        logger = loggerFactory.CreateLogger<StreamHandler>();
        VideoStreamDto = videoStreamDto;
        M3UFileId = videoStreamDto.M3UFileId;
        ProcessId = processId;
        StreamUrl = videoStreamDto.User_Url;
        VideoStreamId = videoStreamDto.Id;
        VideoStreamName = videoStreamDto.User_Tvg_name;

        BoundedChannelOptions options = new(videoBufferSize)
        {
            Capacity = videoBufferSize,
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = true
        };

        //videoChannel = Channel.CreateBounded<Memory<byte>>(options);

        _writeLogger = loggerFactory.CreateLogger<WriteLogger>();
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
            --ClientCount;

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unregistering stream configuration for client {ClientId} {name}", ClientId, VideoStreamName);
            return false;
        }
    }

    private void KillProcess()
    {

        foreach (Process process in Process.GetProcesses())
        {
            if (process.Id == ProcessId)
            {
                process.Kill();
            }
        }

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