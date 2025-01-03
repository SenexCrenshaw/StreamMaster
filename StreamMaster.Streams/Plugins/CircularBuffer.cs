using System.Buffers;

namespace StreamMaster.Streams.Plugins;

/// <summary>
/// A thread-safe circular buffer for storing and managing byte data.
/// </summary>
public class CircularBuffer
{
    private readonly byte[] _buffer;
    private int _start;
    private int _end;
    private readonly Lock _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularBuffer"/> class with the specified size.
    /// </summary>
    /// <param name="size">The capacity of the circular buffer.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the size is less than or equal to zero.</exception>
    public CircularBuffer(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than zero.");
        }

        _buffer = new byte[size];
    }

    /// <summary>
    /// Writes data into the circular buffer.
    /// Overwrites the oldest data if the buffer is full.
    /// </summary>
    /// <param name="data">The data to write.</param>
    public void Write(ReadOnlySpan<byte> data)
    {
        lock (_lock)
        {
            foreach (byte b in data)
            {
                _buffer[_end] = b;
                _end = (_end + 1) % _buffer.Length;

                if (Size == _buffer.Length)
                {
                    // Overwrite the oldest data
                    _start = (_start + 1) % _buffer.Length;
                }
                else
                {
                    Size++;
                }
            }
        }
    }

    /// <summary>
    /// Reads all available data from the circular buffer.
    /// </summary>
    /// <returns>A <see cref="ReadOnlyMemory{Byte}"/> containing the data.</returns>
    public ReadOnlyMemory<byte> ReadAll()
    {
        lock (_lock)
        {
            if (Size == 0)
            {
                return ReadOnlyMemory<byte>.Empty; // No data in the buffer
            }

            if (_end > _start)
            {
                // Simple case: no wrapping
                return new ReadOnlyMemory<byte>(_buffer, _start, Size);
            }
            else
            {
                // Wrapping case: combine the wrapped data into a single memory block
                byte[] combinedBuffer = ArrayPool<byte>.Shared.Rent(Size);
                try
                {
                    int firstPart = _buffer.Length - _start; // From _start to end of buffer
                    Array.Copy(_buffer, _start, combinedBuffer, 0, firstPart);
                    Array.Copy(_buffer, 0, combinedBuffer, firstPart, _end);
                    return new ReadOnlyMemory<byte>(combinedBuffer, 0, Size);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(combinedBuffer);
                }
            }
        }
    }

    /// <summary>
    /// Marks the specified number of bytes as read, making space for new writes.
    /// </summary>
    /// <param name="count">The number of bytes to mark as read.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the count is less than or equal to zero, or greater than the current size.
    /// </exception>
    public void MarkRead(int count)
    {
        lock (_lock)
        {
            if (count <= 0 || count > Size)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Invalid count for marking data as read.");
            }

            _start = (_start + count) % _buffer.Length;
            Size -= count;
        }
    }

    /// <summary>
    /// Gets the current size of the circular buffer (number of bytes stored).
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Gets the total capacity of the circular buffer.
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    /// Gets a value indicating whether the buffer is full.
    /// </summary>
    public bool IsFull => Size == Capacity;
}
