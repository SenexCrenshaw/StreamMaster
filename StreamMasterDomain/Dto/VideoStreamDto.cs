using AutoMapper.Configuration.Annotations;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class VideoStreamDto : BaseVideoStreamDto, IMapFrom<VideoStream>
{
    [Ignore]
    public List<ChildVideoStreamDto> ChildVideoStreams { get; set; }
}
