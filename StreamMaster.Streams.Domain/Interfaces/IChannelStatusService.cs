namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IChannelStatusService
    {

        Task<IChannelStatus> NewChannelStatus(SMChannelDto smChannel);
    }
}