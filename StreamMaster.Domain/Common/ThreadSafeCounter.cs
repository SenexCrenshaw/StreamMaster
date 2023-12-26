namespace StreamMaster.Domain.Common;
public class ThreadSafeCounter
{
    private int count;

    public int Count
    {
        get { return Volatile.Read(ref count); }
    }

    public void Decrement()
    {
        Interlocked.Decrement(ref count);
    }

    public void Increment()
    {
        Interlocked.Increment(ref count);
    }
}
