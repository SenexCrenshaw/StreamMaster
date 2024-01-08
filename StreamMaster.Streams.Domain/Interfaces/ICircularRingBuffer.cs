namespace StreamMaster.Streams.Domain.Interfaces;

public interface ICircularRingBuffer : IDisposable
{
    void IncrementClient();
    void DecrementClient();
    long GetNextReadIndex();
    VideoInfo? VideoInfo { get; set; }
    int BufferSize { get; }
    Guid Id { get; }
    public string VideoStreamId { get; }
    public string VideoStreamName { get; }
    CancellationTokenSource StopVideoStreamingToken { get; set; }

    Task<int> ReadChunkMemory(long index, Memory<byte> target, CancellationToken cancellationToken);

    int WriteChunk(Memory<byte> data);

}
