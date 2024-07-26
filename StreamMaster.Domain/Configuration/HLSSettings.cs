namespace StreamMaster.Domain.Configuration;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class HLSSettings
{
    public bool HLSM3U8Enable { get; set; } = false;
    public string HLSFFMPEGOptions { get; set; } = "-re -i {streamUrl} -copyts -flags +global_header -reconnect 1 -reconnect_at_eof 1 -reconnect_streamed 1 -reconnect_on_network_error 1 -reconnect_on_http_error 1 -reconnect_delay_max 4 -c:a copy -c:v h264_nvenc -preset fast -fps_mode passthrough -y -nostats -hide_banner -f hls -hls_segment_type mpegts -hls_allow_cache 0 -hls_flags temp_file -hls_flags +omit_endlist -hls_flags +discont_start -hls_flags +delete_segments -hls_flags +split_by_time -hls_time 4 -hls_list_size 20 -hls_delete_threshold 40";
    public int HLSReconnectDurationInSeconds { get; set; } = 4;
    public int HLSSegmentDurationInSeconds { get; set; } = 2;
    public int HLSSegmentCount { get; set; } = 10;
    public int HLSM3U8CreationTimeOutInSeconds { get; set; } = 10;
    public int HLSM3U8ReadTimeOutInSeconds { get; set; } = 24;
    public int HLSTSReadTimeOutInSeconds { get; set; } = 4;
}
