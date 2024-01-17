namespace StreamMaster.Domain.Extensions;

public class ConcurrentHashSet<T> : IEnumerable<T>
{
    private readonly HashSet<T> _hashSet = [];
    private readonly object _lock = new();

    public ConcurrentHashSet(IEnumerable<T>? collection = null)
    {
        if (collection is null)
        {
            _hashSet = [];
        }
        else
        {
            // Initialize the HashSet with the provided collection.
            // Note that this operation is not thread-safe; it's expected to be done at construction time only.
            _hashSet = new HashSet<T>(collection);
        }
    }

    public void UnionWith(IEnumerable<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        lock (_lock)
        {
            foreach (T item in other)
            {
                _hashSet.Add(item);
            }
        }
    }

    public bool Add(T item)
    {
        lock (_lock)
        {
            return _hashSet.Add(item);
        }
    }

    public bool Remove(T item)
    {
        lock (_lock)
        {
            return _hashSet.Remove(item);
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _hashSet.Count;
            }
        }
    }

    public bool Contains(T item)
    {
        lock (_lock)
        {
            return _hashSet.Contains(item);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock)
        {
            // To provide a snapshot of the current state of the collection,
            // we create a new HashSet from the original one.
            HashSet<T> snapshot = new(_hashSet);
            return snapshot.GetEnumerator();
        }
    }

    // Explicit implementation for the non-generic interface
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
