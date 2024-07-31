namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface ISwitchToNextStreamService
    {
        Task<bool> SetNextStreamAsync(IStreamStatus ChannelStatus, string? overrideSMStreamId = null);
    }
}