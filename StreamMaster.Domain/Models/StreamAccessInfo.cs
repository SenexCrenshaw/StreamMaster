namespace StreamMaster.Domain.Models;
public class StreamAccessInfo
{
    public DateTime LastAccessTime { get; set; }
    public TimeSpan InactiveThreshold { get; set; }
}
