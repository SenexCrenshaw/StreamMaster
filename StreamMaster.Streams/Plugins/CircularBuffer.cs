
namespace StreamMaster.Streams.Plugins;

public class CircularBuffer
{
    private readonly byte[] _buffer;
    private int _start;
    private int _end;
    private readonly Lock _lock = new();

    public CircularBuffer(int size)
    {
        _buffer = new byte[size];
    }

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
                    // Overwrite oldest data
                    _start = (_start + 1) % _buffer.Length;
                }
                else
                {
                    Size++;
                }
            }
        }
    }

    public byte[] ReadAll()
    {
        lock (_lock)
        {
            byte[] result = new byte[Size];
            if (Size == 0)
            {
                return result; // No data in the buffer
            }

            if (_end > _start)
            {
                // Simple case: no wrapping
                Array.Copy(_buffer, _start, result, 0, Size);
            }
            else
            {
                // Wrapping case
                int firstPart = _buffer.Length - _start; // From _start to end of buffer
                Array.Copy(_buffer, _start, result, 0, firstPart);

                int secondPart = _end; // From start of buffer to _end
                if (secondPart > 0)
                {
                    Array.Copy(_buffer, 0, result, firstPart, secondPart);
                }
            }

            return result;
        }
    }

    public int Size { get; private set; }
    public int Capacity => _buffer.Length;
}
