using StreamMaster.Domain.Attributes;

using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

[RequireAll]
public class FFMPEGProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column(TypeName = "citext")]
    public string Name { get; set; } = "HLS";

    [Column(TypeName = "citext")]
    public string Parameters { get; set; } = "-i {streamUrl} -start_at_zero -copyts -flags +global_header -reconnect 1 -reconnect_at_eof 1 -reconnect_streamed 1 -reconnect_on_network_error 1 -reconnect_on_http_error 1 -reconnect_delay_max 4096 -c:a copy -c:v copy -fps_mode passthrough -y -nostats -hide_banner -f hls -hls_segment_type mpegts -hls_init_time 1 -hls_allow_cache 0 -hls_flags temp_file -hls_flags +omit_endlist -hls_flags +discont_start -hls_flags +delete_segments -hls_flags +split_by_time";

    public int Timeout { get; set; } = 20;

    public bool IsM3U8 { get; set; } = true;

}