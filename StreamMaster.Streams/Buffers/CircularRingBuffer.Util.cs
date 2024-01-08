namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;


/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed partial class CircularRingBuffer : ICircularRingBuffer
{
    public void IncrementClient()
    {
        _inputStreamStatistics.IncrementClient();
    }

    public void DecrementClient()
    {
        _inputStreamStatistics.DecrementClient();
    }

    private int GetAvailableBytes(long readIndex, Guid correlationId)
    {
        long availableBytes = WriteBytes - readIndex;

        return (int)availableBytes;
    }
}
