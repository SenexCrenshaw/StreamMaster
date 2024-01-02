namespace StreamMaster.Streams.Domain.Models;

public class ProxyStreamError
{
    public ProxyStreamErrorCode ErrorCode { get; set; }
    public string? Message { get; set; }
}
