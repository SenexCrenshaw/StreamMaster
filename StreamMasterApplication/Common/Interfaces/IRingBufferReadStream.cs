namespace StreamMasterApplication.Common.Interfaces
{
    public interface IRingBufferReadStream
    {
        ICircularRingBuffer Buffer { get; }
        bool CanRead { get; }
        bool CanSeek { get; }
        bool CanWrite { get; }
        long Length { get; }
        long Position { get; set; }

        void Flush();

        int Read(byte[] buffer, int offset, int count);

        Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        long Seek(long offset, SeekOrigin origin);

        void SetBufferDelegate(Func<ICircularRingBuffer> newBufferDelegate);

        void SetLength(long value);

        void Write(byte[] buffer, int offset, int count);
    }
}
