namespace StreamMasterDomain.Common;

using System.Collections.Concurrent;

public class ThreadSafeIntList
{
    private readonly ConcurrentBag<int> intList;
    private readonly int startingValue;

    public ThreadSafeIntList(int startingValue)
    {
        intList = new ConcurrentBag<int>();
        this.startingValue = startingValue;
    }

    public void AddInt(int value)
    {
        intList.Add(value);
    }

    public bool ContainsInt(int value)
    {
        return intList.Contains(value);
    }

    public int GetNextInt(int? customStartingValue = null)
    {
        int startingInt = customStartingValue ?? startingValue;
        int largestInt = startingInt - 1;
        int missingInt = startingInt;

        foreach (int i in intList)
        {
            if (i > largestInt)
                largestInt = i;

            if (i == missingInt)
                missingInt++;
        }

        intList.Add(missingInt);

        return missingInt;
    }
}
