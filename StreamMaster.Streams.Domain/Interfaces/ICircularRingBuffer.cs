﻿namespace StreamMaster.Streams.Domain.Interfaces;

public interface ICircularRingBuffer : IDisposable
{
    VideoInfo? VideoInfo { get; set; }
    Memory<byte> GetBufferSlice(int length);
    //Task WaitForDataAvailability(Guid clientId, CancellationToken cancellationToken);
    int BufferSize { get; }
    //event EventHandler<Guid> DataAvailable;
    //float GetBufferUtilization();
    /// <summary>
    /// Gets the  ID.
    /// </summary>
    Guid Id { get; }
    public string VideoStreamId { get; }
    public string VideoStreamName { get; }
    CancellationTokenSource StopVideoStreamingToken { get; set; }

    List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

    ICollection<Guid> GetClientIds();

    int GetReadIndex(Guid clientId);

    bool IsPreBuffered();
    //Task<byte> Read(Guid clientId, CancellationToken cancellationToken);

    Task<int> ReadChunkMemory(ulong index, Memory<byte> target, CancellationToken cancellationToken);
    //Task<int> ReadChunk(Guid clientId, byte[] buffer, int offset, int count, CancellationToken cancellationToken);

    void RegisterClient(IClientStreamerConfiguration streamerConfiguration);

    void UnRegisterClient(Guid clientId);

    //void UpdateReadIndex(Guid clientId, int newIndex);

    //Task WaitSemaphoreAsync(Guid clientId, CancellationToken cancellationToken);

    //void Write(byte data);

    Task<int> WriteChunk(Memory<byte> data, CancellationToken cancellationToken);
    //int WriteChunk(byte[] data, int count);
}
