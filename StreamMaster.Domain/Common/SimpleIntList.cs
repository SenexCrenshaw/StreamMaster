using StreamMaster.Domain.Extensions;

namespace StreamMaster.Domain.Common;

public class SimpleIntList(int startingValue)
{
    private readonly ConcurrentHashSet<int> intSet = [];
    private int nextAvailableInt = startingValue;

    public void AddInt(int value)
    {
        intSet.Add(value);
    }

    public bool ContainsInt(int value)
    {
        return intSet.Contains(value);
    }

    private readonly object lockObject = new();

    public int GetNextInt(int? value = null, int? index = null)
    {
        lock (lockObject)
        {
            int desiredValue;
            if (index.HasValue)
            {
                // Use index as base for the desiredValue when provided
                desiredValue = index.Value; // Assuming channel numbers start from 1
                desiredValue = Math.Max(desiredValue, startingValue);

                while (intSet.Contains(desiredValue))
                {
                    desiredValue++;
                }
            }
            else
            {
                // Use the default logic when index is not provided
                desiredValue = value ?? ++nextAvailableInt;
                desiredValue = Math.Max(desiredValue, startingValue);

                while (intSet.Contains(desiredValue))
                {
                    desiredValue = ++nextAvailableInt;
                }
            }

            intSet.Add(desiredValue);
            return desiredValue;
        }
    }


}
