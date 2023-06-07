using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class StreamGroupDto : IMapFrom<StreamGroup>
{
    public List<ChannelGroupDto> ChannelGroups { get; set; } = new();
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StreamGroupNumber { get; set; }
    public List<VideoStreamDto> VideoStreams { get; set; } = new();
}
