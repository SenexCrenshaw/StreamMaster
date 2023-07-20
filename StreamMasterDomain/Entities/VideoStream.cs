using AutoMapper.Configuration.Annotations;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Entities;

public class VideoStream : BaseEntity, IMapFrom<VideoStreamDto>, IMapFrom<ChildVideoStreamDto>
{
    public VideoStream()
    {
        StreamGroups = new List<StreamGroupVideoStream>();
    }

    [Ignore]
    public ICollection<VideoStreamLink> ChildVideoStreams { get; set; }

    public string CUID { get; set; } = string.Empty;

    public int FilePosition { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public bool IsHidden { get; set; } = false;

    public bool IsReadOnly { get; set; } = false;

    public bool IsUserCreated { get; set; } = false;

    public int M3UFileId { get; set; } = 0;

    public ICollection<StreamGroupVideoStream> StreamGroups { get; set; }

    public StreamingProxyTypes StreamProxyType { get; set; } = StreamingProxyTypes.SystemDefault;

    public int Tvg_chno { get; set; } = 0;

    public string Tvg_group { get; set; } = "All";

    public string Tvg_ID { get; set; } = "Dummy";

    public string Tvg_logo { get; set; } = string.Empty;

    public string Tvg_name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public int User_Tvg_chno { get; set; } = 0;

    public string User_Tvg_group { get; set; } = "All";

    public string User_Tvg_ID { get; set; } = "Dummy";

    public string User_Tvg_logo { get; set; } = string.Empty;

    public string User_Tvg_name { get; set; } = string.Empty;

    public string User_Url { get; set; } = string.Empty;

    public VideoStreamHandlers VideoStreamHandler { get; set; } = VideoStreamHandlers.SystemDefault;
}
