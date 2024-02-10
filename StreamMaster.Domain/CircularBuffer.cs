namespace StreamMaster.Domain;

public class CircularBuffer
{
    private readonly byte[] buffer;
    private int writePosition = 0;
    private readonly object lockObj = new();

    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than 0", nameof(capacity));
        }

        Capacity = capacity;
        buffer = new byte[capacity];
    }

    public void Write(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        lock (lockObj)
        {
            int dataLength = data.Length;
            if (dataLength > Capacity)
            {
                // If data is larger than the capacity, write only the last part of data that fits.
                Array.Copy(data, dataLength - Capacity, buffer, 0, Capacity);
                writePosition = 0;
                AvailableData = Capacity;
            }
            else
            {
                int part1Len = Math.Min(Capacity - writePosition, dataLength);
                Array.Copy(data, 0, buffer, writePosition, part1Len);

                if (part1Len < dataLength)
                {
                    // If the data was split, write the remaining part at the beginning of the buffer.
                    Array.Copy(data, part1Len, buffer, 0, dataLength - part1Len);
                }

                writePosition = (writePosition + dataLength) % Capacity;
                AvailableData = Math.Min(AvailableData + dataLength, Capacity);
            }
        }
    }

    public byte[] ReadLatestData()
    {
        lock (lockObj)
        {
            byte[] latestData = new byte[AvailableData]; // Prepare an array to hold the latest data.

            if (AvailableData == 0)
            {
                // No data available to read.
                return latestData;
            }

            if (writePosition == 0 || AvailableData == Capacity)
            {
                // If writePosition is 0, it means the buffer has just wrapped around or is exactly full,
                // so the latest data is the entire buffer.
                Array.Copy(buffer, 0, latestData, 0, AvailableData);
            }
            else
            {
                // Calculate the start position for the latest data that's not contiguous.
                int startIdx = writePosition - AvailableData;
                if (startIdx < 0)
                {
                    // The data wraps around; copy the end segment and then the start segment.
                    startIdx += Capacity; // Correct the start index to a positive value.
                    int part1Length = Capacity - startIdx;
                    Array.Copy(buffer, startIdx, latestData, 0, part1Length);
                    Array.Copy(buffer, 0, latestData, part1Length, writePosition);
                }
                else
                {
                    // All available data is contiguous and can be copied directly.
                    Array.Copy(buffer, startIdx, latestData, 0, AvailableData);
                }
            }

            return latestData;
        }
    }


    public int Capacity { get; }
    public int AvailableData { get; private set; } = 0;
}
