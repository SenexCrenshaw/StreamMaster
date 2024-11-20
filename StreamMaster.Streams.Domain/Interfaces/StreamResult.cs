namespace StreamMaster.Streams.Domain.Interfaces;
public class StreamResult
{
    public Stream? Stream { get; set; }
    public IClientConfiguration? ClientConfiguration { get; set; }
    public string? RedirectUrl { get; set; }
}
