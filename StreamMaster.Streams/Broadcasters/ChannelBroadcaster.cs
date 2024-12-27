using System.Collections.Concurrent;

using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams.Broadcasters;

/// <summary>
/// Manages the broadcasting of a channel to multiple clients.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ChannelBroadcaster"/> class.
/// </remarks>
/// <param name="logger">The logger for logging events.</param>
/// <param name="smChannelDto">The channel data transfer object.</param>
/// <param name="streamGroupProfileId">The stream group profile ID.</param>
public sealed class ChannelBroadcaster(ILogger<IChannelBroadcaster> logger, IOptionsMonitor<Setting> settings, SMChannelDto smChannelDto, int streamGroupProfileId)
    : IChannelBroadcaster, IDisposable
{
    //private ISourceBroadcaster? SourceBroadcaster;
    private readonly Dubcer? Dubcer = null;

    /// <summary>
    /// Event triggered when the channel broadcaster is stopped.
    /// </summary>
    //public event EventHandler<ChannelBroadcasterStopped>? OnChannelBroadcasterStoppedEvent;

    /// <summary>
    /// Gets the dictionary of clients connected to this channel.
    /// </summary>
    public ConcurrentDictionary<string, IClientConfiguration> Clients { get; } = new();

    /// <summary>
    /// Gets a value indicating whether there are any client configurations, excluding "VideoInfo".
    /// </summary>
    public bool ClientConfigurationsEmpty => Clients.IsEmpty || !Clients.Keys.Any(a => a != "VideoInfo");

    /// <summary>
    /// Gets the unique identifier for the channel.
    /// </summary>
    public int Id => SMChannel.Id;

    /// <summary>
    /// Gets the name of the channel.
    /// </summary>
    public string Name => SMChannel.Name;

    /// <summary>
    /// Adds a new client configuration to the channel.
    /// </summary>
    /// <param name="clientConfiguration">The client configuration to add.</param>
    public void AddChannelStreamer(IClientConfiguration clientConfiguration)
    {
        Clients.TryAdd(clientConfiguration.UniqueRequestId, clientConfiguration);
    }

    /// <inheritdoc/>
    public async Task StreamDataToClientsAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        List<IClientConfiguration> clientSnapshot = [.. Clients.Values];
        using SemaphoreSlim semaphore = new(25); // Limit to 25 concurrent streams
        IEnumerable<Task> tasks = clientSnapshot.Select(async client =>
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await StreamDataToClientAsync(client, buffer, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        });

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while streaming data to one or more clients.");
        }
    }

    private async Task StreamDataToClientAsync(IClientConfiguration client, ReadOnlyMemory<byte> buffer, CancellationToken globalCancellationToken)
    {
        // Create a linked cancellation token source, only if timeout is required
        using CancellationTokenSource? clientCts = settings.CurrentValue.ClientReadTimeoutMs > 0
            ? CancellationTokenSource.CreateLinkedTokenSource(globalCancellationToken)
            : null;

        // Apply timeout if configured
        clientCts?.CancelAfter(settings.CurrentValue.ClientReadTimeoutMs);

        try
        {
            if (client.IsStopped)
            {
                return;
            }

            if (client.Pipe.Writer.UnflushedBytes > 10 * 1024 * 1024)
            {
                logger.LogWarning("Client {ClientId} is too slow. Dropping client.", client.UniqueRequestId);
                client.Stop();
                Clients.TryRemove(client.UniqueRequestId, out _);
                return;
            }

            // Use the appropriate cancellation token
            CancellationToken cancellationToken = clientCts?.Token ?? globalCancellationToken;

            // Stream data to the client
            await client.Pipe.Writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            await client.Pipe.Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (clientCts?.Token.IsCancellationRequested == true)
        {
            logger.LogInformation("Streaming to client {ClientId} canceled due to timeout.", client.UniqueRequestId);
            client.Stop();
            Clients.TryRemove(client.UniqueRequestId, out _);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error writing to client {ClientId}. Stopping the client.", client.UniqueRequestId);
            client.Stop();
            Clients.TryRemove(client.UniqueRequestId, out _);
        }
    }

    /// <summary>
    /// Removes a client from the channel.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client to remove.</param>
    /// <returns>True if the client was successfully removed; otherwise, false.</returns>
    public bool RemoveClient(string clientId)
    {
        if (Clients.TryRemove(clientId, out IClientConfiguration? client))
        {
            client.Stop();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stops the channel broadcaster and triggers the stopped event.
    /// </summary>
    public void Stop()
    {
        logger.LogInformation("Channel Broadcaster stopped: {Name}", Name);
        Dubcer?.Stop();
        //OnChannelBroadcasterStoppedEvent?.Invoke(this, new ChannelBroadcasterStopped(Id, Name));
    }

    /// <summary>
    /// Sets the stream information for the channel.
    /// </summary>
    /// <param name="smStreamInfo">The stream information.</param>
    public void SetSMStreamInfo(SMStreamInfo? smStreamInfo)
    {
        if (smStreamInfo == null)
        {
            logger.LogDebug("SetSMStreamInfo null");
        }
        else
        {
            logger.LogDebug("SetSMStreamInfo: {Id} {Name} {Url}", smStreamInfo.Id, smStreamInfo.Name, smStreamInfo.Url);
        }

        SMStreamInfo = smStreamInfo;
    }

    /// <summary>
    /// Marks the channel broadcaster as global.
    /// </summary>
    public void SetIsGlobal()
    {
        IsGlobal = true;
    }

    public int IntroIndex { get; set; }
    public bool PlayedIntro { get; set; }
    public bool IsFirst { get; set; } = true;
    public int StreamGroupProfileId { get; } = streamGroupProfileId;
    public SMChannelDto SMChannel { get; } = smChannelDto;
    public bool Shutdown { get; set; } = false;
    public SMStreamInfo? SMStreamInfo { get; private set; }
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public string? OverrideSMStreamId { get; set; } = null;
    public CustomPlayList? CustomPlayList { get; set; }
    public string SourceName => SMStreamInfo?.Name ?? "";
    public bool IsFailed { get; } = false;

    /// <inheritdoc/>
    public void Dispose()
    {
        Stop();
    }
}
