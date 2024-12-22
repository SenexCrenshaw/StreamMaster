using System.Diagnostics;

using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Factories;

/// <summary>
/// Factory for creating and managing streams.
/// </summary>
public sealed class StreamFactory(
    ILogger<StreamFactory> logger,
    IHTTPStream httpStream,
    ICommandExecutor commandExecutor,
    IProfileService profileService,
    ICustomPlayListStream customPlayListStream,
    IMultiViewPlayListStream multiViewPlayListStream,
    IOptionsMonitor<Setting> settings) : IStreamFactory
{
    /// <inheritdoc/>
    public async Task<GetStreamResult> GetStream(SMStreamInfo SMStreamInfo, CancellationToken cancellationToken)
    {
        GetStreamResult result = await InternalGetStream(SMStreamInfo, cancellationToken).ConfigureAwait(false);

        if (result.Error != null)
        {
            logger.LogError("Error getting stream for {StreamName}: {ErrorMessage}", SMStreamInfo?.Name, result.Error.Message);
        }

        return result;
    }

    public async Task<GetStreamResult> GetMultiViewPlayListStream(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
    {
        GetStreamResult result = await multiViewPlayListStream.HandleStream(channelBroadcaster, cancellationToken).ConfigureAwait(false);

        if (result.Error != null)
        {
            logger.LogError("Error getting stream for {StreamName}: {ErrorMessage}", channelBroadcaster.SMStreamInfo?.Name, result.Error.Message);
        }

        return result;
    }

    private async Task<GetStreamResult> InternalGetStream(SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            if (smStreamInfo == null)
            {
                return new GetStreamResult(null, default, null) { Error = new ProxyStreamError { Message = "SMStreamInfo is null" } };
            }

            string clientUserAgent = !string.IsNullOrEmpty(smStreamInfo.ClientUserAgent)
                ? smStreamInfo.ClientUserAgent
                : settings.CurrentValue.ClientUserAgent;

            return smStreamInfo.SMStreamType switch
            {
                SMStreamTypeEnum.CustomPlayList or SMStreamTypeEnum.Intro or SMStreamTypeEnum.Message
                    => await customPlayListStream.HandleStream(smStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false),

                _ when smStreamInfo.Url.EndsWith(".m3u8")
                    => ExecuteCommandForM3U8(smStreamInfo, clientUserAgent, cancellationToken),

                _ when smStreamInfo.CommandProfile.Command.EqualsIgnoreCase("streammaster")
                    => await httpStream.HandleStream(smStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false),

                _ => ExecuteCommandForCustomProfile(smStreamInfo, clientUserAgent, cancellationToken)
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or Exception)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = ex.Message };
            logger.LogError(ex, "GetStream Error for {StreamName}: {ErrorMessage}", smStreamInfo.Name, error.Message);
            return new GetStreamResult(null, default, null) { Error = error };
        }
        finally
        {
            stopwatch.Stop();
        }
    }
    private GetStreamResult ExecuteCommandForM3U8(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        CommandProfileDto commandProfileDto = profileService.GetM3U8OutputProfile(smStreamInfo.Id);
        logger.LogInformation("Stream URL has m3u8 extension, using {ProfileName} for streaming: {StreamName}", commandProfileDto.ProfileName, smStreamInfo.Name);

        return commandExecutor.ExecuteCommand(commandProfileDto, smStreamInfo.Url, clientUserAgent, null, cancellationToken);
    }

    private GetStreamResult ExecuteCommandForCustomProfile(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Using Command Profile {ProfileName} for streaming: {StreamName}", smStreamInfo.CommandProfile.ProfileName, smStreamInfo.Name);

        return commandExecutor.ExecuteCommand(smStreamInfo.CommandProfile, smStreamInfo.Url, clientUserAgent, null, cancellationToken);
    }
}
