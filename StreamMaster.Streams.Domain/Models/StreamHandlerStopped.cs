namespace StreamMaster.Streams.Domain.Models;

public class StreamHandlerStopped
{
    public string StreamUrl { get; set; }
    public bool InputStreamError { get; set; }
}
