using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;
using StreamMasterDomain.Repository;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class StreamGroupDto : IMapFrom<StreamGroup>
{
    public List<ChannelGroupDto> ChannelGroups { get; set; } = new();
    public List<VideoStreamDto> ChildVideoStreams { get; set; } = new();
    public string HDHRLink { get; set; } = string.Empty;
    public int Id { get; set; }
    public string M3ULink { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int StreamGroupNumber { get; set; }
    public string XMLLink { get; set; } = string.Empty;
}
