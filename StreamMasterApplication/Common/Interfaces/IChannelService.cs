namespace StreamMasterApplication.Common.Interfaces
{
    public interface IChannelService
    {
        IChannelStatus? GetChannelStatus(string videoStreamId);
        int GetGlobalStreamsCount();
        bool HasChannel(string videoStreamId);
        IChannelStatus RegisterChannel(string videoStreamId, string videoStreamName);
        void UnregisterChannel(string videoStreamId);
    }
}