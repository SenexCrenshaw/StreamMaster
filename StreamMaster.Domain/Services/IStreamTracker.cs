namespace StreamMaster.Domain.Services
{
    public interface IStreamTracker
    {
        bool HasStream(int smChannelId);
        bool AddStream(int smChannelId);
        bool RemoveStream(int smChannelId);
    }
}