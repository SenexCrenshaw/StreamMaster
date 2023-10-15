using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Events;

using System.Diagnostics;

namespace StreamMasterInfrastructure.VideoStreamManager;

/// <summary>
/// Manages the streaming of a single video stream, including client registrations and buffer handling.
/// <param name="streamUrl">The URL of the video stream.</param>
/// <param name="logger">The logger instance for logging purposes.</param>
/// <param name="clientManager">The client manager to handle client-specific operations.</param>
/// <param name="buffer">The circular ring buffer for the video stream.</param>
/// <param name="streamingTask">The task that represents the streaming operation.</param>
/// <param name="m3uFileId">The M3U file identifier.</param>
/// <param name="maxStreams">The maximum number of concurrent streams allowed.</param>
/// <param name="processId">The process identifier associated with the stream.</param>
/// <param name="cancellationTokenSource">The cancellation token source for the streaming task.</param>
/// </summary>

public class StreamController : IDisposable, IStreamController
{
    public StreamController() { }

    public StreamController(string streamUrl, IClientStreamerManager clientManager, ICircularRingBuffer buffer, Task streamingTask, int m3uFileId, int maxStreams, int processId, CancellationTokenSource cancellationTokenSource)
    {
        StreamUrl = streamUrl;
        VideoStreamingCancellationToken = cancellationTokenSource;
        StreamingTask = streamingTask;
        RingBuffer = buffer;
        M3UFileId = m3uFileId;
        MaxStreams = maxStreams;
        ProcessId = processId;
        M3UStream = false;
        _clientManager = clientManager;
    }

    public IClientStreamerManager _clientManager { get; set; }

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

    public int ClientCount => _clientManager.ClientCount;
    public bool FailoverInProgress { get; set; }

    public int M3UFileId { get; set; }

    public bool M3UStream { get; set; }

    public int MaxStreams { get; set; }

    public int ProcessId { get; set; } = -1;

    public ICircularRingBuffer RingBuffer { get; private set; }

    public Task StreamingTask { get; set; }
    public string StreamUrl { get; set; }
    public CancellationTokenSource VideoStreamingCancellationToken { get; set; }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    public ClientStreamerConfiguration? GetClientStreamerConfiguration(Guid ClientId)
    {
        return _clientManager.GetClientConfiguration(ClientId);
    }

    public List<ClientStreamerConfiguration> GetClientStreamerConfigurations()
    {
        return _clientManager.GetAllClientConfigurations().ToList();
    }

    public void RegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration)
    {
        try
        {
            _clientManager.RegisterClientConfiguration(streamerConfiguration);

            RingBuffer.RegisterClient(streamerConfiguration.ClientId, streamerConfiguration.ClientUserAgent, streamerConfiguration.ClientIPAddress);
            SetClientBufferDelegate(streamerConfiguration, () => RingBuffer);

            // Raise event
            ClientRegistered?.Invoke(this, new ClientRegisteredEventArgs(streamerConfiguration.ClientId));
        }
        catch (Exception ex)
        {
            // logger.LogError(ex, "Error registering stream configuration for client {ClientId}.", streamerConfiguration.ClientId);
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
                string? procName = CheckProcessExists(ProcessId);
                if (procName != null)
                {
                    Process process = Process.GetProcessById(ProcessId);
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                // logger.LogError(ex, "Error killing process {ProcessId}.", ProcessId);
            }
        }
        OnStreamControllerStopped();
    }

    public bool UnRegisterClientStreamer(ClientStreamerConfiguration streamerConfiguration)
    {
        try
        {
            RingBuffer.UnregisterClient(streamerConfiguration.ClientId);
            bool result = _clientManager.UnregisterClientConfiguration(streamerConfiguration.ClientId);

            // Raise event
            ClientUnregistered?.Invoke(this, new ClientUnregisteredEventArgs(streamerConfiguration.ClientId));
            return result;
        }
        catch (Exception ex)
        {
            // logger.LogError(ex, "Error unregistering stream configuration for client {ClientId}.", streamerConfiguration.ClientId);
            return false;
        }
    }

    private string? CheckProcessExists(int processId)
    {
        try
        {
            Process process = Process.GetProcessById(processId);
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
