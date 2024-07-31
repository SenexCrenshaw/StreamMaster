namespace StreamMaster.Streams.Streams;

public class CommandStream(ILogger<CommandStream> logger, IHTTPStream HTTPStream) : ICommandStream
{
    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(SMStreamInfo SMStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        //if (smStreamDto.Url.EndsWith(".m3u8"))
        //{
        //    //if (profileSettings.CurrentValue.CommandProfiles.TryGetValue("FFMPEG", out CommandProfile? ffmpegProfile))
        //    //{
        //    //    logger.LogInformation("Stream URL has HLS content, using {command} for streaming: {streamName}", ffmpegProfile.Command, channelStatus.SMStream.SourceName);
        //    //    return commandExecutor.ExecuteCommand(ffmpegProfile.Command, ffmpegProfile.Parameters, channelStatus.SMStream.Id, channelStatus.SMStream.Url, clientUserAgent, cancellationToken);
        //    //}
        //    logger.LogInformation("Stream URL has HLS content, using FFMpeg for streaming: {StreamUrl} {streamName}", smStreamDto.Url, smStreamDto.SourceName);
        //    return commandExecutor.ExecuteCommand("FFPMEG", BuildInfo.FFMPEGDefaultOptions, smStreamDto.Id, smStreamDto.Url, clientUserAgent, cancellationToken);
        //}

        //if (channelStatus.CommandProfile.ProfileName != "StreamMaster")
        //{
        //    logger.LogInformation("Using {command} for streaming: {streamName}", channelStatus.CommandProfile.Command, channelStatus.SMStream.SourceName);
        //    return commandExecutor.ExecuteCommand(channelStatus.CommandProfile, channelStatus.SMStream.Url, clientUserAgent, cancellationToken);
        //}

        return await HTTPStream.HandleStream(SMStreamInfo, clientUserAgent, cancellationToken).ConfigureAwait(false);
    }
}
