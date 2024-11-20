namespace StreamMaster.Streams.Domain.Events;

public class StreamTimedOut(string uniqueRequestId, DateTime timeOfTimeout) : EventArgs
{
    public string UniqueRequestId { get; } = uniqueRequestId;
    public DateTime TimeOfTimeout { get; } = timeOfTimeout;
}
