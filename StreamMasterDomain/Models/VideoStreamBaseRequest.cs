using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Models;

public class VideoStreamBaseRequest

{
    public StreamingProxyTypes? StreamingProxyType { get; set; }
    public bool? ToggleVisibility { get; set; }

    //public bool? IsHidden { get; set; }
    public int? Tvg_chno { get; set; }

    public string? Tvg_group { get; set; }
    public string? TimeShift { get; set; }
    public string? Tvg_ID { get; set; }

    public string? Tvg_logo { get; set; }

    public string? Tvg_name { get; set; }

    public string? Url { get; set; }

    public List<VideoStreamDto>? VideoStreams { get; set; }
}