using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Service for recording and managing stream handling metrics.
/// </summary>
public interface IStreamMetricsRecorder
{
    /// <summary>
    /// Gets the metrics associated with stream handling.
    /// </summary>
    StreamHandlerMetrics Metrics { get; }

    /// <summary>
    /// Records metrics for a stream operation, including the number of bytes read and latency.
    /// </summary>
    /// <param name="bytesRead">The number of bytes read during the operation.</param>
    /// <param name="latency">The latency in milliseconds.</param>
    void RecordMetrics(int bytesRead, double latency);

    /// <summary>
    /// Records metrics for a timed operation.
    /// </summary>
    /// <param name="action">The asynchronous operation to measure.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The number of bytes processed, if applicable.</returns>
    Task<int> RecordMetricsAsync(Func<ValueTask<int>> action, CancellationToken cancellationToken);
}
