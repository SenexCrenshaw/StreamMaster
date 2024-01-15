using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class StreamGroupDto : IMapFrom<StreamGroup>
{
    public bool IsLoading { get; set; } = false;
    //public List<ChannelGroupDto> ChannelGroups { get; set; } = new();
    //public List<VideoStreamDto> ChildVideoStreams { get; set; } = new();
    public string HDHRLink { get; set; } = string.Empty;
    public bool IsReadOnly { get; set; } = false;
    public bool AutoSetChannelNumbers { get; set; } = false;
    public int StreamCount { get; set; } = 0;

    public int Id { get; set; }
    public string M3ULink { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    //public int StreamGroupNumber { get; set; }
    public string XMLLink { get; set; } = string.Empty;
    public string ShortLink { get; set; } = string.Empty;
}
