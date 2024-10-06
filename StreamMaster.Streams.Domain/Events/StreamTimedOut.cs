namespace StreamMaster.Streams.Domain.Events;

public class StreamTimedOut : EventArgs
{
    public string UniqueRequestId { get; }
    public DateTime TimeOfTimeout { get; }

    public StreamTimedOut(string uniqueRequestId, DateTime timeOfTimeout)
    {
        UniqueRequestId = uniqueRequestId;
        TimeOfTimeout = timeOfTimeout;
    }
}
