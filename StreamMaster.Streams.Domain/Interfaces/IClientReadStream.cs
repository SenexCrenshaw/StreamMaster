using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides an interface for reading from a ring buffer stream.
/// </summary>
public interface IClientReadStream : IDisposable, IStreamStats
{
    //IByteTrackingChannel Channel { get; }
    Channel<byte[]> Channel { get; }

    /// <summary>
    /// Gets a value indicating whether the stream supports reading.
    /// </summary>
    bool CanRead { get; }

    /// <summary>
    /// Gets a value indicating whether the stream supports seeking.
    /// </summary>
    bool CanSeek { get; }

    /// <summary>
    /// Gets a value indicating whether the stream supports writing.
    /// </summary>
    bool CanWrite { get; }

    /// <summary>
    /// Gets the length of the stream.
    /// </summary>
    long Length { get; }

    /// <summary>
    /// Gets or sets the position within the stream.
    /// </summary>
    long Position { get; set; }

    /// <summary>
    /// Gets the unique identifier for the stream.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    void Flush();

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream.
    /// </summary>
    int Read(byte[] buffer, int offset, int count);

    /// <summary>
    /// Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream, and monitors cancellation requests.
    /// </summary>
    Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Sets the position within the current stream.
    /// </summary>
    long Seek(long offset, SeekOrigin origin);

    /// <summary>
    /// Sets the length of the current stream.
    /// </summary>
    void SetLength(long value);

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position within this stream.
    /// </summary>
    void Write(byte[] buffer, int offset, int count);

    void Cancel();
}
