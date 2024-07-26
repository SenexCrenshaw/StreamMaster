namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface ISwitchToNextStreamService
    {
        Task<bool> SetNextStreamAsync(IM3U8ChannelStatus ChannelStatus, string? overrideSMStreamId = null);
    }
}