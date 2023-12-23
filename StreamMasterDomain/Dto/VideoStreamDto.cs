using AutoMapper.Configuration.Annotations;



using StreamMasterDomain.Attributes;
using StreamMasterDomain.Models;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class VideoStreamDto : BaseVideoStreamDto, IMapFrom<VideoStream>
{
    public int MaxStreams { get; set; } = 0;
    public bool IsLoading { get; set; } = false;
    public int ChannelGroupId { get; set; } = 0;
    public int Rank { get; set; } = 0;

    [Ignore]
    public List<VideoStreamDto> ChildVideoStreams { get; set; } = [];
}
