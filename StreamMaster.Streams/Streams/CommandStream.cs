using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Streams;

public class CommandStream(ILogger<CommandStream> logger, IHTTPStream HTTPStream, IOptionsMonitor<CommandProfileList> profileSettings, ICommandExecutor commandExecutor) : ICommandStream
{
    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(IChannelStatus channelStatus, string clientUserAgent, CancellationToken cancellationToken)
    {
        if (channelStatus.SMStream.Url.EndsWith(".m3u8"))
        {
            if (profileSettings.CurrentValue.CommandProfiles.TryGetValue("FFMPEG", out CommandProfile? ffmpegProfile))
            {
                logger.LogInformation("Stream URL has HLS content, using {command} for streaming: {streamName}", ffmpegProfile.Command, channelStatus.SMStream.Name);
                return commandExecutor.ExecuteCommand(ffmpegProfile.Command, ffmpegProfile.Parameters, channelStatus.SMStream.Id, channelStatus.SMStream.Url, clientUserAgent, cancellationToken);
            }
            logger.LogInformation("Stream URL has HLS content, using FFMpeg for streaming: {StreamUrl} {streamName}", channelStatus.SMStream.Url, channelStatus.SMStream.Name);
            return commandExecutor.ExecuteCommand(channelStatus.CommandProfile, channelStatus.SMStream.Url, clientUserAgent, cancellationToken);
        }

        if (channelStatus.CommandProfile.ProfileName != "StreamMaster")
        {
            logger.LogInformation("Using {command} for streaming: {streamName}", channelStatus.CommandProfile.Command, channelStatus.SMStream.Name);
            return commandExecutor.ExecuteCommand(channelStatus.CommandProfile, channelStatus.SMStream.Url, clientUserAgent, cancellationToken);
        }

        return await HTTPStream.HandleStream(channelStatus, clientUserAgent, cancellationToken).ConfigureAwait(false);
    }
}
