namespace StreamMaster.Streams.Streams;

public class CommandStream(IHTTPStream HTTPStream) : ICommandStream
{
    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(SMStreamInfo SMStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        return await HTTPStream.HandleStream(SMStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false);
    }
}
