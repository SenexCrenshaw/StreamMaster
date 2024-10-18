namespace StreamMaster.Domain.Models;
public class StreamAccessInfo
{
    public required string Key { get; set; }
    public required DateTime LastAccessTime { get; set; }
    public required TimeSpan InactiveThreshold { get; set; }
    public required string SMStreamId { get; set; }
    public required double MillisecondsSinceLastUpdate { get; set; }
}
