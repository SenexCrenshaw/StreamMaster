using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface ISMStreamChannelBase<T>
{
    bool CanPeek { get; }
    Task<bool> WriteAsync(T item, CancellationToken cancellationToken = default);
    ValueTask<T> ReadAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default);
    void Complete();
}

public interface IByteTrackingChannel<T> : ISMStreamChannelBase<T>
{
    long CurrentByteSize { get; }
    ChannelReader<byte[]> Reader { get; }
}

public interface ISMRegularChannel<T> : ISMStreamChannelBase<T>;