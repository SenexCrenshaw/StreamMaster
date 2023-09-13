using AutoMapper.Configuration.Annotations;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class VideoStreamDto : BaseVideoStreamDto, IMapFrom<VideoStream>
{
    public bool IsLoading { get; set; } = false;
    public int ChannelGroupId { get; set; } = 0;
    public int Rank { get; set; } = 0;

    [Ignore]
    public List<ChildVideoStreamDto> ChildVideoStreams { get; set; }
}
