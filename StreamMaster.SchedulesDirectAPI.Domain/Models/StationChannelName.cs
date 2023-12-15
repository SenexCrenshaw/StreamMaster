using System.ComponentModel.DataAnnotations;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class StationChannelName
{
    [Required]
    public string Id => Channel;
    [Required]
    public required string Channel { get; set; }
    [Required]
    public required string ChannelName { get; set; }
    [Required]
    public required string DisplayName { get; set; }
}
