using StreamMaster.PlayList;

namespace StreamMaster.Streams.Streams;

public class CustomPlayListStream(ILogger<CustomPlayListStream> logger, ICustomPlayListBuilder customPlayListBuilder, ICommandExecutor commandExecutor) : ICustomPlayListStream
{
    public const string CustomPlayListFFMpegOptions = "-hide_banner -loglevel error -ss {secondsIn} -re -i {streamUrl} -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1";

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(IChannelStatus channelStatus, string clientUserAgent, CancellationToken cancellationToken)
    {
        if (channelStatus.CustomPlayList == null)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"Custom playlist not found: {channelStatus.SMChannel.Name}" };
            logger.LogError("GetProxyStream Error: {message}", error.Message);
            return (null, -1, error);
        }

        (string videoFileName, int secondsIn) = GetCustomStreamFileName(channelStatus);
        string options = CustomPlayListFFMpegOptions.Replace("{secondsIn}", secondsIn.ToString());
        return commandExecutor.ExecuteCommand("ffmpeg", options, channelStatus.SMStream.Id, videoFileName, clientUserAgent, cancellationToken);
    }

    private (string videoFileName, int secondsIn) GetCustomStreamFileName(IChannelStatus channelStatus)
    {
        CustomStreamNfo? intro = customPlayListBuilder.GetIntro(channelStatus.IntroIndex);
        if (intro != null && !channelStatus.PlayedIntro)
        {
            channelStatus.PlayedIntro = true;
            channelStatus.IntroIndex++;
            if (channelStatus.IntroIndex >= customPlayListBuilder.IntroCount)
            {
                channelStatus.IntroIndex = 0;
            }

            return (intro.VideoFileName, 0);
        }

        channelStatus.PlayedIntro = false;
        if (channelStatus.IsFirst)
        {
            channelStatus.IsFirst = false;
            int secondsIn;
            (CustomStreamNfo StreamNfo, secondsIn) = customPlayListBuilder.GetCurrentVideoAndElapsedSeconds(channelStatus.CustomPlayList.Name);
            return (StreamNfo.VideoFileName, secondsIn);
        }
        else
        {
            (CustomStreamNfo StreamNfo, _) = customPlayListBuilder.GetCurrentVideoAndElapsedSeconds(channelStatus.CustomPlayList.Name);
            return (StreamNfo.VideoFileName, 0);
        }
    }
}
