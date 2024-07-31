namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IChannelStatusService
    {
        IChannelStatus NewChannelStatus(SMChannelDto smChannel);
    }
}