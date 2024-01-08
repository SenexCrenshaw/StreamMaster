namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IInputStatisticsManager
    {
        List<InputStreamingStatistics> GetAllInputStreamStatistics();
        IInputStreamingStatistics RegisterInputReader(StreamInfo StreamInfo);
        public IInputStreamingStatistics? GetInputStreamStatistics(string videoStreamId);
    }
}