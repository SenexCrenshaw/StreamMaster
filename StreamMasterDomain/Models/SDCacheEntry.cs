namespace StreamMasterDomain.Models;

public class SDCacheEntry<T>
{
    public DateTime Timestamp { get; set; }
    public string Command { get; set; }
    public string Content { get; set; }
    public T Data { get; set; }
}
