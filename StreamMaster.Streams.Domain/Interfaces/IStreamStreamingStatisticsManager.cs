using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamStreamingStatisticsManager
    {
        void AddBytesRead(string SMStreamId, int bytesRead);
        List<string> GetAllClientIds();
        List<StreamStreamingStatistic> GetStreamingStatistics();
        void IncrementBytesRead(string SMStreamId);
        StreamStreamingStatistic RegisterStream(SMStreamDto smStream);
        bool UnRegisterStream(string SMStreamId);
    }
}