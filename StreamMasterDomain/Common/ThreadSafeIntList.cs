using System.Collections.Concurrent;
namespace StreamMasterDomain.Common;

public class ThreadSafeIntList
{
    private readonly ConcurrentDictionary<int, bool> intSet;
    private readonly int startingValue;
    private int nextAvailableInt;

    public ThreadSafeIntList(int startingValue)
    {
        intSet = new ConcurrentDictionary<int, bool>();
        this.startingValue = startingValue;
        nextAvailableInt = startingValue;
    }

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
