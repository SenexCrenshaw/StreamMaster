namespace StreamMaster.Domain.Models;

public class SDCacheEntry<T>
{
    public DateTime Timestamp { get; set; }
    public required string Command { get; set; }
    public required string Content { get; set; }
    public required T Data { get; set; }
}
