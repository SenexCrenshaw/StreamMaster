namespace StreamMaster.Domain
{
    public class CircularBuffer
    {
        private readonly byte[] buffer;
        private int writePosition = 0;
        private readonly object lockObj = new();

        /// <summary>
        /// Initializes a new instance of the CircularBuffer class with a specified capacity.
        /// </summary>
        /// <param name="capacity">The maximum amount of data the buffer can hold.</param>
        /// <exception cref="ArgumentException">Thrown when capacity is less than or equal to 0.</exception>
        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be greater than 0.", nameof(capacity));
            }

            Capacity = capacity;
            buffer = new byte[capacity];
        }

        /// <summary>
        /// Writes data into the circular buffer. If the buffer does not have enough space,
        /// older data will be overwritten.
        /// </summary>
        /// <param name="data">The byte array to write into the buffer.</param>
        /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
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
                    // Overwrite the buffer with the last 'Capacity' bytes of 'data'
                    Array.Copy(data, dataLength - Capacity, buffer, 0, Capacity);
                    writePosition = 0;
                    AvailableData = Capacity;
                }
                else
                {
                    // Calculate how much data can be written without wrapping
                    int part1Len = Math.Min(Capacity - writePosition, dataLength);
                    Array.Copy(data, 0, buffer, writePosition, part1Len);

                    if (part1Len < dataLength)
                    {
                        // Wrap and write the remaining data
                        Array.Copy(data, part1Len, buffer, 0, dataLength - part1Len);
                    }

                    // Update write position and available data, wrapping the write position if necessary
                    writePosition = (writePosition + dataLength) % Capacity;
                    AvailableData = Math.Min(AvailableData + dataLength, Capacity);
                }
            }
        }

        /// <summary>
        /// Reads the most recent data written into the buffer without removing it.
        /// </summary>
        /// <returns>A byte array containing the latest data. The size of the array is up to the amount of available data.</returns>
        public byte[] ReadLatestData()
        {
            lock (lockObj)
            {
                byte[] latestData = new byte[AvailableData];

                if (AvailableData == 0)
                {
                    return latestData; // Early exit if no data available
                }

                if (writePosition == 0 || AvailableData == Capacity)
                {
                    // Buffer is exactly full, or just wrapped around
                    Array.Copy(buffer, 0, latestData, 0, AvailableData);
                }
                else
                {
                    // Data wraps around the buffer end; copy in two segments
                    int startIdx = (Capacity + writePosition - AvailableData) % Capacity;
                    int part1Length = Math.Min(AvailableData, Capacity - startIdx);
                    Array.Copy(buffer, startIdx, latestData, 0, part1Length);
                    if (part1Length < AvailableData)
                    {
                        Array.Copy(buffer, 0, latestData, part1Length, AvailableData - part1Length);
                    }
                }

                return latestData;
            }
        }
        /// <summary>
        /// Gets the capacity of the buffer.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Gets the amount of data currently stored in the buffer.
        /// </summary>
        public int AvailableData { get; private set; } = 0;

        /// <summary>
        /// Clears all data from the buffer, resetting its state.
        /// </summary>
        public void Clear()
        {
            lock (lockObj)
            {
                writePosition = 0;
                AvailableData = 0;
            }
        }

    }
}
