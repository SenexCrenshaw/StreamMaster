namespace StreamMasterApplication.Common.Interfaces
{
    public interface IStreamSwitcher
    {
        Task<bool> SwitchToNextVideoStreamAsync(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null);
    }
}