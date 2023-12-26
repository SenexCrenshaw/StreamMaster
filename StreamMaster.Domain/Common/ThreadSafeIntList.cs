using System.Collections.Concurrent;
namespace StreamMaster.Domain.Common;

public class ThreadSafeIntList(int startingValue)
{
    private readonly ConcurrentDictionary<int, bool> intSet = new();
    private int nextAvailableInt = startingValue;

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
        desiredValue = Math.Max(desiredValue, startingValue);

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
