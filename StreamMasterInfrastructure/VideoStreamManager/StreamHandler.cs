using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Events;

using System.Diagnostics;

namespace StreamMasterInfrastructure.VideoStreamManager;

/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// <param name="streamUrl">The URL of the video stream.</param>
/// <param name="logger">The logger instance for logging purposes.</param>
/// <param name="clientManager">The client manager to handle client-specific operations.</param>
/// <param name="buffer">The circular ring circularRingbuffer for the video stream.</param>
/// <param name="streamingTask">The task that represents the streaming operation.</param>
/// <param name="m3uFileId">The M3U file identifier.</param>
/// <param name="maxStreams">The maximum number of concurrent streams allowed.</param>
/// <param name="processId">The process identifier associated with the stream.</param>
/// <param name="cancellationTokenSource">The cancellation token source for the streaming task.</param>
/// </summary>
public class StreamHandler : IDisposable, IStreamHandler
{
    private readonly IClientStreamerManager _clientStreamerManager;

    private readonly ILogger<IStreamHandler> _logger;
    private StreamHandler(
        ILogger<IStreamHandler> logger,
        IClientStreamerManager clientStreamerManager,
        int processId,
        ICircularRingBuffer ringBuffer,
        CancellationTokenSource cancellationTokenSource)
    {
        _logger = logger;
        _clientStreamerManager = clientStreamerManager;
        VideoStreamingCancellationToken = cancellationTokenSource;
        RingBuffer = ringBuffer;
        ProcessId = processId;
    }

    public static async Task<IStreamHandler?> CreateAsync(
        ILogger<IStreamHandler> logger,
        ChildVideoStreamDto childVideoStreamDto,
        int rank,
        IMemoryCache memoryCache,
        ICircularRingBufferFactory circularRingBufferFactory,
        IClientStreamerManager clientStreamerManager
    )
    {
        CancellationTokenSource cancellationTokenSource = new();

        ICircularRingBuffer ringBuffer = circularRingBufferFactory.CreateCircularRingBuffer(childVideoStreamDto, rank);

        (Stream? stream, int processId, ProxyStreamError? error) = await GetProxy(childVideoStreamDto.User_Url, logger, memoryCache, cancellationTokenSource.Token);
        if (stream == null || error != null)
        {
            return null;
        }

        StreamHandler controller = new(logger, clientStreamerManager, processId, ringBuffer, cancellationTokenSource);

        _ = controller.StartVideoStreamingAsync(stream, ringBuffer);
        return controller;
    }

    private static void LogErrorIfAny(ILogger _logger, Stream? stream, ProxyStreamError? error, string streamUrl)
    {
        if (stream == null || error != null)
        {
            _logger.LogError("Error getting proxy stream for {StreamUrl}: {Error?.Message}", streamUrl, error?.Message);
        }
    }

    private static async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(string streamUrl, ILogger<IStreamHandler> logger, IMemoryCache memoryCache, CancellationToken cancellationToken)
    {
        Setting setting = memoryCache.GetSetting();

        Stream? stream;
        ProxyStreamError? error;
        int processId;

        if (setting.StreamingProxyType == StreamingProxyTypes.FFMpeg)
        {
            (stream, processId, error) = await StreamingProxies.GetFFMpegStream(streamUrl, logger, setting);
            LogErrorIfAny(logger, stream, error, streamUrl);
        }
        else
        {
            (stream, processId, error) = await StreamingProxies.GetProxyStream(streamUrl, logger, setting, cancellationToken);
            LogErrorIfAny(logger, stream, error, streamUrl);
        }

        return (stream, processId, error);
    }

    private async Task DelayWithCancellation(int milliseconds, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(milliseconds, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task was cancelled");
            throw;
        }
    }

    private async Task LogRetryAndDelay(int retryCount, int maxRetries, int waitTime, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            _logger.LogInformation("Stream was cancelled for: {StreamUrl}", StreamUrl);
        }

        _logger.LogInformation("Stream received 0 bytes for stream: {StreamUrl} Retry {retryCount}/{maxRetries}",
            StreamUrl,
            retryCount,
            maxRetries);

        await DelayWithCancellation(waitTime, token);
    }

    private async Task StartVideoStreamingAsync(Stream stream, ICircularRingBuffer circularRingbuffer)
    {
        const int chunkSize = 24 * 1024;

        _logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream: {StreamUrl}", chunkSize, StreamUrl);

        Memory<byte> bufferMemory = new byte[chunkSize];

        const int maxRetries = 3; //setting.MaxConnectRetry > 0 ? setting.MaxConnectRetry : 3;
        const int waitTime = 50;// setting.MaxConnectRetryTimeMS > 0 ? setting.MaxConnectRetryTimeMS : 50;

        using (stream)
        {
            int retryCount = 0;
            while (!VideoStreamingCancellationToken.IsCancellationRequested && retryCount < maxRetries)
            {
                try
                {
                    int bytesRead = await TryReadStream(bufferMemory, stream, VideoStreamingCancellationToken.Token);
                    if (bytesRead == -1)
                    {
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        retryCount++;
                        await LogRetryAndDelay(retryCount, maxRetries, waitTime, VideoStreamingCancellationToken.Token);
                    }
                    else
                    {
                        circularRingbuffer.WriteChunk(bufferMemory[..bytesRead]);
                        retryCount = 0;
                    }
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogError(ex, "Stream cancelled for: {StreamUrl}", StreamUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Stream error for: {StreamUrl}", StreamUrl);
                    break;
                }
            }
        }

        _logger.LogInformation("Stream stopped for: {StreamUrl}", StreamUrl);
        if (!VideoStreamingCancellationToken.IsCancellationRequested)
        {
            VideoStreamingCancellationToken.Cancel();
        }
    }

    private async Task<int> TryReadStream(Memory<byte> bufferChunk, Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            if (!stream.CanRead || cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Stream is not readable or cancelled");
                return -1;
            }

            return await stream.ReadAsync(bufferChunk, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            return -1;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Raised when an error occurs during stream operations.
    /// </summary>
    public event EventHandler<StreamFailedEventArgs>? StreamFailed;

    /// <summary>
    /// Raised when a new client is registered.
    /// </summary>
    public event EventHandler<ClientRegisteredEventArgs>? ClientRegistered;

    /// <summary>
    /// Raised when a client is unregistered.
    /// </summary>
    public event EventHandler<ClientUnregisteredEventArgs>? ClientUnregistered;

    /// <summary>
    /// Raised when a stream stops.
    /// </summary>
    public event EventHandler<StreamControllerStoppedEventArgs>? StreamControllerStopped;

    public int ClientCount => _clientStreamerManager.ClientCount;
    public bool FailoverInProgress { get; set; }

    public int M3UFileId { get; set; }

    public bool M3UStream { get; set; }

    public int MaxStreams { get; set; }

    public int ProcessId { get; set; } = -1;

    public ICircularRingBuffer RingBuffer { get; }
    public string StreamUrl { get; set; }
    public CancellationTokenSource VideoStreamingCancellationToken { get; set; }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    public ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid ClientId)
    {
        return _clientStreamerManager.GetClientConfiguration(ClientId);
    }

    public List<ClientStreamerConfiguration> GetClientStreamerConfigurations()
    {
        return _clientStreamerManager.GetAllClientConfigurations().ToList();
    }

    public void RegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration)
    {
        try
        {
            _clientStreamerManager.RegisterClientConfiguration(streamerConfiguration);

            RingBuffer.RegisterClient(streamerConfiguration.ClientId, streamerConfiguration.ClientUserAgent, streamerConfiguration.ClientIPAddress);
            SetClientBufferDelegate(streamerConfiguration, () => RingBuffer);

            // Raise event
            ClientRegistered?.Invoke(this, new ClientRegisteredEventArgs(streamerConfiguration.ClientId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering stream configuration for client {ClientId}.", streamerConfiguration.ClientId);
            StreamFailed?.Invoke(this, new StreamFailedEventArgs($"Failed to register client {streamerConfiguration.ClientId}."));
        }
    }

    protected virtual void OnStreamControllerStopped()
    {
        StreamControllerStopped?.Invoke(this, new());
    }

    public void Stop()
    {
        if (VideoStreamingCancellationToken?.IsCancellationRequested == false)
        {
            VideoStreamingCancellationToken.Cancel();
        }

        if (ProcessId > 0)
        {
            try
            {
                string? procName = CheckProcessExists();
                if (procName != null)
                {
                    Process process = Process.GetProcessById(ProcessId);
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error killing process {ProcessId}.", ProcessId);
            }
        }
        OnStreamControllerStopped();
    }

    public bool UnRegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration)
    {
        try
        {
            RingBuffer.UnregisterClient(streamerConfiguration.ClientId);
            bool result = _clientStreamerManager.UnregisterClientConfiguration(streamerConfiguration.ClientId);

            // Raise event
            ClientUnregistered?.Invoke(this, new ClientUnregisteredEventArgs(streamerConfiguration.ClientId));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering stream configuration for client {ClientId}.", streamerConfiguration.ClientId);
            return false;
        }
    }

    private string? CheckProcessExists()
    {
        try
        {
            Process process = Process.GetProcessById(ProcessId);
            // logger.LogInformation($"Process with ID {processId} exists. Name: {process.ProcessName}");
            return process.ProcessName;
        }
        catch (ArgumentException)
        {
            // logger.LogWarning($"Process with ID {processId} does not exist.");
            return null;
        }
    }

    private void SetClientBufferDelegate(ClientStreamerConfiguration config, Func<ICircularRingBuffer> func)
    {
        ClientStreamerConfiguration? sc = GetClientStreamerConfiguration(config.ClientId);
        if (sc is null || sc.ReadBuffer is null)
        {
            return;
        }

        sc.ReadBuffer.SetBufferDelegate(func, sc);
    }
}
