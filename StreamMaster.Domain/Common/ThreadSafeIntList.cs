using System.Collections.Concurrent;
namespace StreamMaster.Domain.Common;

public class ThreadSafeIntList(int nextAvailableInt)
{
    private readonly ConcurrentDictionary<int, bool> intSet = new();

    public void AddInt(int value)
    {
        _ = intSet.TryAdd(value, true);
    }

    public bool ContainsInt(int value)
    {
        return intSet.ContainsKey(value);
    }

    public int GetNextInt(int? value = null)
    {
        int desiredValue = value ?? Interlocked.Increment(ref nextAvailableInt);

        // Ensure desiredValue is not less than startingValue
        desiredValue = Math.Max(desiredValue, nextAvailableInt);

        while (true)
        {
            if (intSet.TryAdd(desiredValue, true))
            {
                return desiredValue;
            }
            desiredValue = Interlocked.Increment(ref nextAvailableInt);
        }
    }
}
