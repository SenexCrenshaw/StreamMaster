using System.Collections.Concurrent;

namespace StreamMaster.Domain.Extensions;

public static class ConcurrentBagExtensions
{
    public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> range)
    {
        foreach (T? item in range)
        {
            if (item is not null)
            {
                bag.Add(item);
            }
        }
    }
}