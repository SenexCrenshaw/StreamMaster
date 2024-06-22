using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IChannelStreamingStatisticsManager
    {
        List<ChannelStreamingStatistics> GetChannelStreamingStatistics();
        ChannelStreamingStatistics RegisterInputReader(SMChannelDto smChannelDto, int currentRank, string currentStreamId);
        public ChannelStreamingStatistics? GetChannelStreamingStatistic(int smChannelID);
        bool UnRegister(int smChannelID);
        void IncrementClient(int smChannelID);
        void DecrementClient(int smChannelID);
    }
}