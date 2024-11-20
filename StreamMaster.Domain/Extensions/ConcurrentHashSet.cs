using System.Collections;

namespace StreamMaster.Domain.Extensions;

/// <summary>
/// Represents a thread-safe set of values.
/// </summary>
/// <typeparam name="T">The type of elements in the hash set.</typeparam>
/// <remarks>
/// Initializes a new instance of the ConcurrentHashSet class that is empty or contains elements copied from the specified collection.
/// </remarks>
/// <param name="collection">The collection whose elements are copied to the new set.</param>
public class ConcurrentHashSet<T>(IEnumerable<T>? collection = null) : IEnumerable<T>
{
    private readonly HashSet<T> _hashSet = collection is null ? ([]) : [.. collection];
    private readonly Lock _lock = new();

    /// <summary>
    /// Adds all elements in the specified collection to the current set.
    /// </summary>
    /// <param name="other">The collection of items to add to the set.</param>
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

    /// <summary>
    /// Adds the specified element to the set.
    /// </summary>
    /// <param name="item">The element to add to the set.</param>
    /// <returns>true if the element is added to the set; false if the element is already present.</returns>
    public bool Add(T item)
    {
        lock (_lock)
        {
            return _hashSet.Add(item);
        }
    }

    /// <summary>
    /// Removes the specified element from the set.
    /// </summary>
    /// <param name="item">The element to remove.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(T item)
    {
        lock (_lock)
        {
            return _hashSet.Remove(item);
        }
    }

    /// <summary>
    /// Gets the number of elements contained in the set.
    /// </summary>
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

    /// <summary>
    /// Determines whether the set contains a specific value.
    /// </summary>
    /// <param name="item">The object to locate in the set.</param>
    /// <returns>true if the object is found in the set; otherwise, false.</returns>
    public bool Contains(T item)
    {
        lock (_lock)
        {
            return _hashSet.Contains(item);
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the set.
    /// </summary>
    /// <returns>An enumerator for the set.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock)
        {
            HashSet<T> snapshot = [.. _hashSet];
            return snapshot.GetEnumerator();
        }
    }

    /// <summary>
    /// Returns a non-generic enumerator that iterates through the set.
    /// </summary>
    /// <returns>A non-generic enumerator for the set.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
