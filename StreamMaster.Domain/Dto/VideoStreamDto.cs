using AutoMapper.Configuration.Annotations;

using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Models;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class VideoStreamDto : BaseVideoStreamDto, IMapFrom<VideoStream>
{
    public bool IsReadOnly { get; set; } = false;
    public int MaxStreams { get; set; } = 0;
    public bool IsLoading { get; set; } = false;
    public int ChannelGroupId { get; set; } = 0;
    public int Rank { get; set; } = 0;

    [Ignore]
    public List<VideoStreamDto> ChildVideoStreams { get; set; } = [];
}
