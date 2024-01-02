namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IInputStatisticsManager
    {
        IInputStreamingStatistics RegisterReader(string videoStreamId);
        public IInputStreamingStatistics GetInputStreamStatistics(string videoStreamId);
    }
}