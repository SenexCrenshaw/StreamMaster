namespace StreamMaster.Domain.Configuration;

public class SettingsFile<T>
{
    public string FileName { get; set; } = string.Empty;
    public Type SettingType { get; set; }
    public SettingsFile(string fileName, Type settingType)
    {
        FileName = fileName;
        SettingType = settingType;
    }
}

public static class SettingFiles
{
    public static FFMPEGProfiles DefaultProfileSetting = new()
    {
        Profiles = new Dictionary<string, FFMPEGProfile>
        {
            {
                "HLS",
                 new FFMPEGProfile
                 {
                     Parameters = "-i {streamUrl} -start_at_zero -copyts -flags +global_header -reconnect 1 -reconnect_at_eof 1 -reconnect_streamed 1 -reconnect_on_network_error 1 -reconnect_on_http_error 1 -reconnect_delay_max 4096 -c:a copy -c:v copy -fps_mode passthrough -y -nostats -hide_banner -f hls -hls_segment_type mpegts -hls_init_time 1 -hls_allow_cache 0 -hls_flags temp_file -hls_flags +omit_endlist -hls_flags +discont_start -hls_flags +delete_segments -hls_flags +split_by_time",
                     IsM3U8 = true,
                 }
             },
            {
                 "MPEGTS",
                 new FFMPEGProfile
                 {
                     Parameters = "-i {streamUrl} -c copy -f mpegts pipe:1"
                 }
             },
            {
                 "MP4",
                 new FFMPEGProfile
                 {
                     Parameters = "-i {streamUrl} -c copy -f mp4 pipe:1"
                 }
             }
        }
    };
}

