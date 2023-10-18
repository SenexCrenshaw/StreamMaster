using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

public interface ICircularRingBuffer
{
    int BufferSize { get; }

    float GetBufferUtilization();
    /// <summary>
    /// Gets the  ID.
    /// </summary>
    Guid Id { get; }
    public string VideoStreamId { get; }

    List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

    int GetAvailableBytes(Guid clientId);

    ICollection<Guid> GetClientIds();

    int GetReadIndex(Guid clientId);

    bool IsPreBuffered();
    Task<byte> Read(Guid clientId, CancellationToken cancellationToken);

    Task<int> ReadChunkMemory(Guid clientId, Memory<byte> target, CancellationToken cancellationToken);
    Task<int> ReadChunk(Guid clientId, byte[] buffer, int offset, int count, CancellationToken cancellationToken);

    void RegisterClient(Guid clientId, string clientAgent, string clientIPAddress);

    void UnRegisterClient(Guid clientId);

    void UpdateReadIndex(Guid clientId, int newIndex);

    Task WaitSemaphoreAsync(Guid clientId, CancellationToken cancellationToken);

    void Write(byte data);

    int WriteChunk(Memory<byte> data);
    int WriteChunk(byte[] data, int count);
}
