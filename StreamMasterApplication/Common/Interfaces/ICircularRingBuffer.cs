using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces
{
    public interface ICircularRingBuffer
    {
        int BufferSize { get; }

        public int VideoStreamId { get; }

        List<ClientStreamingStatistics> GetAllStatistics();

        List<StreamStatisticsResult> GetAllStatisticsForAllUrls();

        int GetAvailableBytes(Guid clientId);

        ICollection<Guid> GetClientIds();

        StreamingStatistics? GetClientStatistics(Guid clientId);

        StreamingStatistics GetInputStreamStatistics();

        int GetReadIndex(Guid clientId);

        SingleStreamStatisticsResult GetSingleStreamStatisticsResult();

        bool IsPreBuffered();

        Task<byte> Read(Guid clientId, CancellationToken cancellationToken);

        Task<int> ReadChunk(Guid clientId, byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        void RegisterClient(Guid clientId, string clientAgent);

        void ReleaseSemaphore(Guid clientId);

        void UnregisterClient(Guid clientId);

        void UpdateReadIndex(Guid clientId, int newIndex);

        Task WaitSemaphoreAsync(Guid clientId, CancellationToken cancellationToken);

        void Write(byte data);

        int WriteChunk(byte[] data, int count);
    }
}
