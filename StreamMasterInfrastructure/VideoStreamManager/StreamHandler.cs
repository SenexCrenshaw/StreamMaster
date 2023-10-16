using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Events;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMasterInfrastructure.VideoStreamManager;

/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public class StreamHandler(string streamURL, int processId, ILogger<IStreamHandler> logger, ICircularRingBuffer ringBuffer, CancellationTokenSource cancellationTokenSource) : IStreamHandler
{
    private readonly ConcurrentDictionary<Guid, ClientStreamerConfiguration> _clientStreamerConfigurations = new();

    public bool FailoverInProgress { get; set; }
    public int M3UFileId { get; set; }
    public int ProcessId { get; set; } = processId;
    public ICircularRingBuffer RingBuffer { get; } = ringBuffer;
    public string StreamUrl { get; set; } = streamURL;
    private async Task DelayWithCancellation(int milliseconds)
    {
        try
        {
            await Task.Delay(milliseconds, VideoStreamingCancellationToken.Token);
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Task was cancelled");
            throw;
        }
    }

    private async Task LogRetryAndDelay(int retryCount, int maxRetries, int waitTime)
    {
        if (VideoStreamingCancellationToken.Token.IsCancellationRequested)
        {
            logger.LogInformation("Stream was cancelled for: {StreamUrl}", StreamUrl);
        }

        logger.LogInformation("Stream received 0 bytes for stream: {StreamUrl} Retry {retryCount}/{maxRetries}",
            StreamUrl,
            retryCount,
            maxRetries);

        await DelayWithCancellation(waitTime);
    }

    public async Task StartVideoStreamingAsync(Stream stream, ICircularRingBuffer circularRingbuffer)
    {
        const int chunkSize = 24 * 1024;

        logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream: {StreamUrl}", chunkSize, StreamUrl);

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
                    int bytesRead = await TryReadStream(bufferMemory, stream);
                    if (bytesRead == -1)
                    {
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        retryCount++;
                        await LogRetryAndDelay(retryCount, maxRetries, waitTime);
                    }
                    else
                    {
                        circularRingbuffer.WriteChunk(bufferMemory[..bytesRead]);
                        retryCount = 0;
                    }
                }
                catch (TaskCanceledException ex)
                {
                    logger.LogError(ex, "Stream cancelled for: {StreamUrl}", StreamUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Stream error for: {StreamUrl}", StreamUrl);
                    break;
                }
            }
        }

        logger.LogInformation("Stream stopped for: {StreamUrl}", StreamUrl);
        if (!VideoStreamingCancellationToken.IsCancellationRequested)
        {
            VideoStreamingCancellationToken.Cancel();
        }
    }

    private async Task<int> TryReadStream(Memory<byte> bufferChunk, Stream stream)
    {
        try
        {
            if (!stream.CanRead || VideoStreamingCancellationToken.Token.IsCancellationRequested)
            {
                logger.LogWarning("Stream is not readable or cancelled");
                return -1;
            }

            return await stream.ReadAsync(bufferChunk, VideoStreamingCancellationToken.Token);
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

    public CancellationTokenSource VideoStreamingCancellationToken { get; set; } = cancellationTokenSource;

    public int ClientCount => _clientStreamerConfigurations.Count;

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    private void RegisterClient(ClientStreamerConfiguration streamerConfiguration)
    {
        _clientStreamerConfigurations.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
        RingBuffer.RegisterClient(streamerConfiguration.ClientId, streamerConfiguration.ClientUserAgent, streamerConfiguration.ClientIPAddress);
    }

    public void RegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration)
    {
        try
        {
            RegisterClient(streamerConfiguration);
            SetClientBufferDelegate(streamerConfiguration, () => RingBuffer);

            // Raise event
            ClientRegistered?.Invoke(this, new ClientRegisteredEventArgs(streamerConfiguration.ClientId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering stream configuration for client {ClientId}.", streamerConfiguration.ClientId);
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
                logger.LogError(ex, "Error killing process {ProcessId}.", ProcessId);
            }
        }
        OnStreamControllerStopped();
    }

    private bool UnRegisterClient(ClientStreamerConfiguration streamerConfiguration)
    {
        bool result = _clientStreamerConfigurations.TryRemove(streamerConfiguration.ClientId, out _);
        RingBuffer.UnregisterClient(streamerConfiguration.ClientId);
        return result;
    }
    public bool UnRegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration)
    {
        try
        {
            bool result = UnRegisterClient(streamerConfiguration);
            // Raise event
            ClientUnregistered?.Invoke(this, new ClientUnregisteredEventArgs(streamerConfiguration.ClientId));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unregistering stream configuration for client {ClientId}.", streamerConfiguration.ClientId);
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

    public ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid clientID)
    {
        if (_clientStreamerConfigurations.TryGetValue(clientID, out ClientStreamerConfiguration? clientConfig))
        {
            return clientConfig;
        }
        throw new Exception($"Client configuration for {clientID} not found");
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

    public ICollection<ClientStreamerConfiguration>? GetClientStreamerConfigurations()
    {
        return _clientStreamerConfigurations.Values;
    }
}
