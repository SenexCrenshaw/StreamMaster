using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text.Json.Serialization;

using MessagePack;

using Microsoft.AspNetCore.Http;

using StreamMaster.Streams.Domain.Statistics;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams;

/// <summary>
/// Represents the configuration for a client, including details such as the client's IP address, user agent, and associated stream channel.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ClientConfiguration"/> class with the specified parameters.
/// </remarks>
/// <param name="uniqueRequestId">The unique request identifier.</param>
/// <param name="smChannel">The SMChannel associated with the client.</param>
/// <param name="clientUserAgent">The client's user agent string.</param>
/// <param name="clientIPAddress">The client's IP address.</param>
/// <param name="response">The HTTP response associated with the client.</param>
/// <param name="loggerFactory">The logger factory for creating loggers.</param>
/// <param name="cancellationToken">The cancellation Token associated with the client.</param>
public class ClientConfiguration(
    string uniqueRequestId,
    SMChannelDto smChannel,
    string clientUserAgent,
    string clientIPAddress,
    HttpResponse response,
    ILoggerFactory loggerFactory,
    IOptionsMonitor<Setting> settings,
    CancellationToken cancellationToken) : IClientConfiguration
{
    private readonly StreamMetricsTracker MetricsService = new();
    public StreamHandlerMetrics Metrics => MetricsService.Metrics;
    public event EventHandler? ClientStopped;
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientConfiguration"/> class for serialization purposes.
    /// </summary>
    //public ClientConfiguration() { }

    /// <inheritdoc/>
    [IgnoreMember]
    public HttpResponse Response { get; } = response;

    public TaskCompletionSource<bool> CompletionSource { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Pipe Pipe { get; set; } = new();
    /// <inheritdoc/>
    public void SetUniqueRequestId(string uniqueRequestId)
    {
        UniqueRequestId = uniqueRequestId;
    }

    protected virtual void OnClientStopped(EventArgs e)
    {
        ClientStopped?.Invoke(this, e);
    }

    public void Stop()
    {
        ////ClientStream?.Write([0], 0, 1);
        ////ClientStream?.Cancel();
        ////ClientStream?.Flush();
        ////ClientStream?.Dispose();
        // Response.CompleteAsync().Wait();
        CancellationTokenSourc.Cancel();
        CompletionSource.TrySetResult(true);
        //OnClientStopped(EventArgs.Empty);
    }

    public ILoggerFactory LoggerFactory { get; set; } = loggerFactory;

    /// <inheritdoc/>
    [IgnoreMember]
    public string HttpContextId => Response.HttpContext.TraceIdentifier;

    /// <inheritdoc/>
    [IgnoreMember]
    [JsonIgnore]
    public CancellationToken ClientCancellationToken { get; set; } = cancellationToken;
    public CancellationTokenSource CancellationTokenSourc { get; } = new();

    /// <inheritdoc/>
    public string UniqueRequestId { get; set; } = uniqueRequestId;

    /// <inheritdoc/>
    public string ClientIPAddress { get; set; } = clientIPAddress;

    /// <inheritdoc/>
    public string ClientUserAgent { get; set; } = clientUserAgent;

    /// <inheritdoc/>
    public SMChannelDto SMChannel { get; set; } = smChannel;

    public async Task StreamFromPipeToResponseAsync()
    {
        Stopwatch stopwatch = new();
        CancellationTokenSource inactivityCts = new();
        Timer? inactivityTimer = null;

        if (settings.CurrentValue.ClientReadTimeOutSeconds > 0)
        {
            inactivityTimer = new Timer(_ =>
            {
                inactivityCts.Cancel(); // Cancel the client if it has been inactive for too long
            }, null, Timeout.Infinite, Timeout.Infinite);
        }

        try
        {
            while (!ClientCancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();

                // Start the inactivity timer only if the timeout setting is greater than 0
                inactivityTimer?.Change(settings.CurrentValue.ClientReadTimeOutSeconds * 1000, Timeout.Infinite);

                // Read data from the Pipe
                ReadResult result = await Pipe.Reader.ReadAsync(ClientCancellationToken).ConfigureAwait(false);
                ReadOnlySequence<byte> buffer = result.Buffer;

                foreach (ReadOnlyMemory<byte> segment in buffer)
                {
                    try
                    {
                        stopwatch.Stop();
                        double latency = stopwatch.Elapsed.TotalMilliseconds;
                        MetricsService.RecordMetrics(segment.Length, latency);
                        await Response.Body.WriteAsync(segment, ClientCancellationToken).ConfigureAwait(false);

                        // Reset the inactivity timer if it exists
                        inactivityTimer?.Change(settings.CurrentValue.ClientReadTimeOutSeconds * 1000, Timeout.Infinite);
                    }
                    catch (Exception )
                    {
                        //LoggerFactory.CreateLogger<ClientConfiguration>().LogWarning(ex, "Failed to write to response for client {ClientId}", UniqueRequestId);
                        Stop(); // Stop the client on failure
                        return;
                    }
                }

                // Mark the buffer as consumed
                Pipe.Reader.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break; // Stop if no more data will be written to the Pipe
                }
            }
        }
        catch (OperationCanceledException) when (ClientCancellationToken.IsCancellationRequested)
        {
            // Graceful cancellation
            LoggerFactory.CreateLogger<ClientConfiguration>().LogInformation("Streaming task canceled for client {ClientId}", UniqueRequestId);
        }
        catch (OperationCanceledException) when (inactivityCts.IsCancellationRequested)
        {
            LoggerFactory.CreateLogger<ClientConfiguration>().LogWarning("Client {ClientId} was inactive for too long and has been canceled.", UniqueRequestId);
            Stop(); // Stop the client due to inactivity
        }
        catch (Exception ex)
        {
            LoggerFactory.CreateLogger<ClientConfiguration>().LogError(ex, "Unexpected error occurred during streaming for client {ClientId}", UniqueRequestId);
        }
        finally
        {
            // Dispose of the inactivity timer if it exists
            inactivityTimer?.Dispose();

            // Complete the Pipe.Reader
            await Pipe.Reader.CompleteAsync().ConfigureAwait(false);

            stopwatch.Stop();
            // Notify that the client has stopped
            Stop();
        }
    }
}
