/// <summary>
/// Defines the contract for creating proxy streams.
/// </summary>
public interface IProxyFactory
{
    /// <summary>
    /// Asynchronously gets a proxy stream based on the provided stream URL.
    /// </summary>
    /// <param name="streamUrl">The URL of the stream to proxy.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a tuple with the following elements:
    /// - <see cref="Stream"/>: The proxy stream. Null if the operation failed.
    /// - <see cref="int"/>: The process ID associated with the stream. -1 if not applicable.
    /// - <see cref="ProxyStreamError"/>: An error object containing details of any error that occurred. Null if the operation was successful.
    /// </returns>
    Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(string streamUrl, CancellationToken cancellationToken);
}
