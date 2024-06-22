namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IInputStreamingStatistics
    {
        void UpdateValues();
        double BitsPerSecond { get; }
        long BytesRead { get; }
        long BytesWritten { get; }
        int ChannelId { get; }
        string ChannelName { get; }
        int Clients { get; }
        string ElapsedTime { get; }
        string Id { get; }

        string? ChannelLogo { get; }

        string StreamId { get; }
        string StreamName { get; }
        string? StreamLogo { get; }

        int Rank { get; }
        DateTimeOffset StartTime { get; }
        StreamInfo StreamInfo { get; }
        string? StreamUrl { get; }

        void AddBytesRead(long bytesRead);
        void AddBytesWritten(long bytesWritten);
        void DecrementClient();
        void IncrementBytesRead();
        void IncrementBytesWritten();
        void IncrementClient();
    }
}