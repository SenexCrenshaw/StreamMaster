using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http;

using StreamMaster.Streams.Domain.Metrics;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams;

/// <summary>
/// Represents the configuration for a client, including details such as the client's IP address, user agent, and associated stream channel.
/// </summary>
public class ClientConfiguration : IClientConfiguration, IDisposable
{
    private readonly StreamMetricsRecorder StreamMetricsRecorder = new();
    private readonly ILogger<ClientConfiguration> logger;
    private readonly CancellationTokenSource inactivityCts = new();
    private readonly Timer? inactivityTimer;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientConfiguration"/> class.
    /// </summary>
    /// <param name="uniqueRequestId">The unique request identifier.</param>
    /// <param name="smChannel">The SMChannel associated with the client.</param>
    /// <param name="clientUserAgent">The client's user agent string.</param>
    /// <param name="clientIPAddress">The client's IP address.</param>
    /// <param name="response">The HTTP response associated with the client.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    /// <param name="settings">The settings monitor for configuration values.</param>
    /// <param name="cancellationToken">The cancellation token associated with the client.</param>
    public ClientConfiguration(
        string uniqueRequestId,
        SMChannelDto smChannel,
        string clientUserAgent,
        string clientIPAddress,
        HttpResponse response,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<Setting> settings,
        CancellationToken cancellationToken)
    {
        UniqueRequestId = uniqueRequestId;
        SMChannel = smChannel;
        ClientUserAgent = clientUserAgent;
        ClientIPAddress = clientIPAddress;
        Response = response;
        LoggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<ClientConfiguration>();
        Settings = settings;
        ClientCancellationToken = cancellationToken;

        // Initialize inactivity timer if timeout is set
        if (settings.CurrentValue.ClientReadTimeoutMs > 0)
        {
            inactivityTimer = new Timer(_ => inactivityCts.Cancel(), null, Timeout.Infinite, Timeout.Infinite);
        }
    }

    public bool IsStopped { get; private set; }

    /// <inheritdoc/>
    public StreamHandlerMetrics Metrics => StreamMetricsRecorder.Metrics;

    /// <inheritdoc/>
    public TaskCompletionSource<bool> ClientCompletionSource { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <inheritdoc/>
    public Pipe Pipe { get; set; } = new();

    /// <inheritdoc/>
    public HttpResponse Response { get; }

    /// <inheritdoc/>
    public string UniqueRequestId { get; }

    /// <inheritdoc/>
    public string ClientIPAddress { get; set; }

    /// <inheritdoc/>
    public string ClientUserAgent { get; set; }

    /// <inheritdoc/>
    public SMChannelDto SMChannel { get; set; }

    /// <inheritdoc/>
    public ILoggerFactory LoggerFactory { get; }

    /// <inheritdoc/>
    [JsonIgnore]
    public CancellationToken ClientCancellationToken { get; set; }

    private IOptionsMonitor<Setting> Settings { get; }

    /// <inheritdoc/>
    public event EventHandler? OnClientStopped;

    private readonly Lock stopLock = new();
    /// <inheritdoc/>
    public void Stop()
    {
        lock (stopLock)
        {
            if (disposed)
            {
                return;
            }

            logger.LogInformation("Stopping client {ClientId}.", UniqueRequestId);

            try
            {
                inactivityTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                ClientCompletionSource.TrySetResult(true);
                //OnClientStopped?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while stopping client {ClientId}.", UniqueRequestId);
            }
            finally
            {
                disposed = true;
            }
        }
    }

    /// <summary>
    /// Streams data from the pipe to the HTTP response asynchronously.
    /// </summary>
    public async Task StreamFromPipeToResponseAsync()
    {
        Stopwatch stopwatch = new();

        try
        {
            while (!ClientCancellationToken.IsCancellationRequested && !IsStopped)
            {
                inactivityTimer?.Change(Settings.CurrentValue.ClientReadTimeoutMs, Timeout.Infinite);

                await StreamMetricsRecorder.RecordMetricsAsync(
                    async () =>
                    {
                        ReadResult result = await Pipe.Reader.ReadAsync(ClientCancellationToken).ConfigureAwait(false);
                        ReadOnlySequence<byte> buffer = result.Buffer;

                        foreach (ReadOnlyMemory<byte> segment in buffer)
                        {
                            await Response.Body.WriteAsync(segment, ClientCancellationToken).ConfigureAwait(false);
                            inactivityTimer?.Change(Settings.CurrentValue.ClientReadTimeoutMs, Timeout.Infinite);
                        }

                        Pipe.Reader.AdvanceTo(buffer.End);

                        if (result.IsCompleted)
                        {
                            // Exit the loop gracefully if the result indicates completion
                            throw new OperationCanceledException("Pipe reading completed.");
                        }

                        return (int)buffer.Length; // Return the total bytes processed
                    },
                    ClientCancellationToken);
            }
        }
        catch (OperationCanceledException) when (ClientCancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Streaming to client {ClientId} stopped.", UniqueRequestId);
        }
        catch (OperationCanceledException) when (inactivityCts.IsCancellationRequested)
        {
            logger.LogWarning("Client {ClientId} inactive for too long and has been canceled.", UniqueRequestId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during streaming for client {ClientId}.", UniqueRequestId);
        }
        finally
        {
            inactivityTimer?.Dispose();
            await Pipe.Reader.CompleteAsync().ConfigureAwait(false);
            stopwatch.Stop();
            OnClientStopped?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Disposes the client configuration, releasing all resources.
    /// </summary>
    public void Dispose()
    {
        IsStopped = true;
        if (disposed)
        {
            return;
        }

        try
        {
            inactivityTimer?.Dispose();
            inactivityCts.Cancel();
            inactivityCts.Dispose();

            Pipe?.Reader.Complete();
            Pipe?.Writer.Complete();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while disposing client configuration for client {ClientId}.", UniqueRequestId);
        }
        finally
        {
            disposed = true;
            logger.LogInformation("Disposed client {ClientId}.", UniqueRequestId);
        }

        GC.SuppressFinalize(this);
    }
}
