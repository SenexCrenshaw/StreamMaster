namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class StationChannelName
{
    public  string Id => Channel;
    public required string Channel { get; set; }
    public required string ChannelName { get; set; }
    public required string DisplayName { get; set; }
}
