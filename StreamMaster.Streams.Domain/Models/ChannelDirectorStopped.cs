namespace StreamMaster.Streams.Domain.Models;

public class ChannelDirectorStopped
{
    public SMStreamInfo SMStreamInfo { get; set; }

    public ChannelDirectorStopped(SMStreamInfo smStreamInfo)
    {
        SMStreamInfo = smStreamInfo;
    }
}
