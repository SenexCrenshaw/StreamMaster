using StreamMaster.Domain.Configuration;

using System.Diagnostics;

namespace StreamMaster.Streams.Factories;

public sealed class ProxyFactory(ILogger<ProxyFactory> logger, ICustomPlayListStream CustomPlayListStream, ICommandStream CommandStream, IOptionsMonitor<Setting> settings)
    : IProxyFactory
{

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await GetProxyStream(channelStatus, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null)
        {
            logger.LogError("Error getting proxy stream for {streamName}: {ErrorMessage}", channelStatus.SMStream.Name, error?.Message);
        }
        return (stream, processId, error);
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxyStream(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {

            string clientUserAgent = !string.IsNullOrEmpty(channelStatus.SMStream.ClientUserAgent) ? channelStatus.SMStream.ClientUserAgent : settings.CurrentValue.SourceClientUserAgent;

            return channelStatus.SMChannel.IsCustomStream
                ? await CustomPlayListStream.HandleStream(channelStatus, clientUserAgent, cancellationToken)
                : await CommandStream.HandleStream(channelStatus, clientUserAgent, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is HttpRequestException or Exception)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = ex.Message };
            logger.LogError(ex, "GetProxyStream Error for {channelStatus.SMStream.Name}", error.Message);
            return (null, -1, error);
        }
        finally
        {
            stopwatch.Stop();
        }
    }

}
