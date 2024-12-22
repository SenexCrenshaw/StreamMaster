namespace StreamMaster.Streams.Streams;

/// <summary>
/// Handles the creation and management of command-based streams.
/// </summary>
public class CommandStream(IHTTPStream httpStream) : ICommandStream
{
    /// <inheritdoc/>
    public async Task<GetStreamResult> HandleStream(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        // Delegate the stream handling to the HTTP stream implementation
        return await httpStream.HandleStream(smStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false);
    }
}
