namespace StreamMaster.Streams.Buffers;

public class CircularBuffer<T>
{
    private readonly T[] buffer;
    private int head;
    private int tail;

    public CircularBuffer(int capacity)
    {
        buffer = new T[capacity];
        head = 0;
        tail = 0;
        Count = 0;
    }

    public void Enqueue(T item)
    {
        buffer[tail] = item;
        if (Count == buffer.Length)
        {
            head = (head + 1) % buffer.Length; // Overwrite
        }
        else
        {
            Count++;
        }
        tail = (tail + 1) % buffer.Length;
    }

    public void Dequeue()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Buffer is empty");
        }
        head = (head + 1) % buffer.Length;
        Count--;
    }

    public int Count { get; private set; }

    public T[] ToArray()
    {
        T[] result = new T[Count];
        for (int i = 0; i < Count; i++)
        {
            result[i] = buffer[(head + i) % buffer.Length];
        }
        return result;
    }

    public void Clear()
    {
        head = 0;
        tail = 0;
        Count = 0;
    }
}
