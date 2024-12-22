using System.IO.Pipelines;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Http;

using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Represents the configuration for a client, including details such as the client's IP address, user agent, and associated stream channel.
/// </summary>
public interface IClientConfiguration : IDisposable
{
    /// <summary>
    /// Gets the stream handler metrics for the client.
    /// </summary>
    StreamHandlerMetrics Metrics { get; }

    /// <summary>
    /// Gets the task completion source for the client.
    /// </summary>
    TaskCompletionSource<bool> ClientCompletionSource { get; }

    /// <summary>
    /// Occurs when the client has stopped.
    /// </summary>
    event EventHandler OnClientStopped;

    /// <summary>
    /// Gets the cancellation token associated with the client.
    /// </summary>
    [JsonIgnore]
    CancellationToken ClientCancellationToken { get; }

    /// <summary>
    /// Gets the SMChannel associated with the client.
    /// </summary>
    SMChannelDto SMChannel { get; }

    /// <summary>
    /// Gets or sets the client's IP address.
    /// </summary>
    string ClientIPAddress { get; set; }

    /// <summary>
    /// Gets or sets the client's user agent string.
    /// </summary>
    string ClientUserAgent { get; set; }

    /// <summary>
    /// Gets the HTTP response associated with the client.
    /// </summary>
    HttpResponse Response { get; }

    /// <summary>
    /// Gets the pipe used for streaming data to the client.
    /// </summary>
    Pipe Pipe { get; }

    /// <summary>
    /// Gets the unique request identifier for the client.
    /// </summary>
    string UniqueRequestId { get; }

    /// <summary>
    /// Stops the client and releases associated resources.
    /// </summary>
    void Stop();
}
