namespace StreamMaster.Domain.Configuration;

public class SettingsFile<T>(string fileName, Type settingType)
{
    public string FileName { get; set; } = fileName;
    public Type SettingType { get; set; } = settingType;
}

public static class SettingFiles
{
    public static OutputProfiles DefaultOutputProfileSetting = new()
    {
        OutProfiles = new Dictionary<string, OutputProfile>
        {
            {
                "Default",
                 new OutputProfile
                 {
                    IsReadOnly=true,

                        EnableIcon=true,
                        EnableId=true,
                        EnableGroupTitle=true,
                        EnableChannelNumber =true,

                        Name=ValidM3USetting.Name.ToString(),
                        Group=ValidM3USetting.Group.ToString(),
                        EPGId=ValidM3USetting.EPGId.ToString(),
                 }
            },

             {
                "PLEX",
                 new OutputProfile
                 {
                      EnableIcon=true,
                        EnableId=true,
                        EnableGroupTitle=true,
                         EnableChannelNumber =true,

                        Name=ValidM3USetting.Name.ToString(),
                        EPGId=ValidM3USetting.EPGId.ToString(),
                        Group=ValidM3USetting.Group.ToString(),

                 }
            },


        }

    };
    public static VideoOutputProfiles DefaultVideoProfileSetting = new()
    {
        VideoProfiles = new Dictionary<string, VideoOutputProfile>
        {
            {
                "HLS",
                 new VideoOutputProfile
                 { IsReadOnly=true,
                     Command="ffmpeg",
                     Parameters = "-i {streamUrl} -start_at_zero -copyts -flags +global_header -reconnect 1 -reconnect_at_eof 1 -reconnect_streamed 1 -reconnect_on_network_error 1 -reconnect_on_http_error 1 -reconnect_delay_max 4096 -c:a copy -c:v copy -fps_mode passthrough -y -nostats -hide_banner -f hls -hls_segment_type mpegts -hls_init_time 1 -hls_allow_cache 0 -hls_flags temp_file -hls_flags +omit_endlist -hls_flags +discont_start -hls_flags +delete_segments -hls_flags +split_by_time",
                     IsM3U8 = true,
                 }
             },
            {
                 "Default",
                 new VideoOutputProfile
                 { IsReadOnly=true,
                     Command="ffmpeg",
                     Parameters = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1"
                 }
             },
            {
                 "MP4",
                 new VideoOutputProfile
                 { IsReadOnly=true,
                     Command="ffmpeg",
                     Parameters = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1"
                 }
             }
        }
    };
}

