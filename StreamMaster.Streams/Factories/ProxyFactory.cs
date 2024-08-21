using StreamMaster.Domain.Enums;

using System.Diagnostics;

namespace StreamMaster.Streams.Factories;

public sealed class ProxyFactory(ILogger<ProxyFactory> logger, IHTTPStream HTTPStream, ICommandExecutor commandExecutor, IProfileService profileService, ICustomPlayListStream CustomPlayListStream, IOptionsMonitor<Setting> settings)
    : IProxyFactory
{
    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await GetProxyStream(smStreamInfo, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null)
        {
            logger.LogError("Error getting proxy stream for {streamName}: {ErrorMessage}", smStreamInfo.Name, error?.Message);
        }
        return (stream, processId, error);
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxyStream(SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            string clientUserAgent = !string.IsNullOrEmpty(smStreamInfo.ClientUserAgent) ? smStreamInfo.ClientUserAgent : settings.CurrentValue.ClientUserAgent;

            if (smStreamInfo.SMStreamType == SMStreamTypeEnum.CustomPlayList)
            {
                return await CustomPlayListStream.HandleStream(smStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false);
            }

            if (smStreamInfo.SMStreamType == SMStreamTypeEnum.Intro)
            {
                return await CustomPlayListStream.HandleStream(smStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false);
            }

            if (smStreamInfo.SMStreamType == SMStreamTypeEnum.Message)
            {
                return await CustomPlayListStream.HandleStream(smStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false);
            }

            if (smStreamInfo.Url.EndsWith(".m3u8"))
            {
                logger.LogInformation("Stream URL has m3u8 extension, using ffmpeg for streaming: {streamName}", smStreamInfo.Name);
                CommandProfileDto commandProfileDto = profileService.GetCommandProfile("SMFFMPEG");
                return commandExecutor.ExecuteCommand(commandProfileDto, smStreamInfo.Url, clientUserAgent, null, cancellationToken);
            }

            //FIX ME : Handler for just commands and not stream master proxy
            return await HTTPStream.HandleStream(smStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false);
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
