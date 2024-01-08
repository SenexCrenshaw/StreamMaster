namespace StreamMaster.Streams.Domain.Interfaces;

public interface ICircularRingBuffer : IDisposable
{
    long GetNextReadIndex();
    VideoInfo? VideoInfo { get; set; }
    int BufferSize { get; }
    Guid Id { get; }
    public string VideoStreamId { get; }
    public string VideoStreamName { get; }
    CancellationTokenSource StopVideoStreamingToken { get; set; }

    //List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

    Task<int> ReadChunkMemory(long index, Memory<byte> target, CancellationToken cancellationToken);

    Task<int> WriteChunk(Memory<byte> data, CancellationToken cancellationToken);

}
