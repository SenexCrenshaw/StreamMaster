namespace StreamMaster.Streams.Domain.Interfaces;

public interface IInputStreamingStatistics
{
    string? StreamUrl { get; }
    string ChannelId { get; }
    public string Id { get; }
    double BitsPerSecond { get; }
    long BytesRead { get; set; }
    long BytesWritten { get; set; }
    public string ElapsedTime { get; }
    DateTimeOffset StartTime { get; set; }
    public int Rank { get; }
    public string ChannelName { get; }
    public string? Logo { get; set; }
    int Clients { get; set; }

    void AddBytesRead(long bytesRead);
    void AddBytesWritten(long bytesWritten);
    void DecrementClient();
    void IncrementBytesRead();
    void IncrementBytesWritten();
    void IncrementClient();
}