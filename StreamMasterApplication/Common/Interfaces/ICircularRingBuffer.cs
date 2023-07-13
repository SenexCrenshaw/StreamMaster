using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface ICircularRingBuffer
    {
        int BufferSize { get; }

        List<ClientStreamingStatistics> GetAllStatistics();
        int GetAvailableBytes(Guid clientId);
        IReadOnlyList<Guid> GetClientIds();
        StreamingStatistics? GetClientStatistics(Guid clientId);
        StreamingStatistics GetInputStreamStatistics();
        int GetReadIndex(Guid clientId);
        bool NewDataAvailable(Guid clientId);
        byte Read(Guid clientId, CancellationToken cancellationToken);
        int ReadChunk(Guid clientId, byte[] buffer, int offset, int count, CancellationToken cancellationToken);
        void RegisterClient(Guid clientId);
        void ReleaseSemaphore(Guid clientId);
        void UnregisterClient(Guid clientId);
        void UpdateReadIndex(Guid clientId, int newIndex);
        Task WaitSemaphoreAsync(Guid clientId, CancellationToken cancellationToken);
        void Write(byte data);
        int WriteChunk(byte[] data, int count);
    }
}