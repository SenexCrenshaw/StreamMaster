namespace StreamMaster.Domain.Services
{
    public interface IStreamTracker
    {
        bool HasStream(string smStreamId);
        bool AddStream(string smStreamId);
        bool RemoveStream(string smStreamId);
    }
}