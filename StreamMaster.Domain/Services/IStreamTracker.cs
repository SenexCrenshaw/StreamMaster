namespace StreamMaster.Domain.Services
{
    public interface IStreamTracker
    {
        bool HasStream(string streamId);
        bool AddStream(string streamId);
        bool RemoveStream(string streamId);
    }
}