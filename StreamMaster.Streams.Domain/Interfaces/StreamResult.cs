namespace StreamMaster.Streams.Domain.Interfaces;
public class StreamResult
{
    public IClientConfiguration? ClientConfiguration { get; set; }
    public string? RedirectUrl { get; set; }
    // Signal when streaming is complete
}
