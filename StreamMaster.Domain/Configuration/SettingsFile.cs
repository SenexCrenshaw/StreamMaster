namespace StreamMaster.Domain.Configuration;

public class SettingsFile<T>(string fileName, Type settingType)
{
    public string FileName { get; set; } = fileName;
    public Type SettingType { get; set; } = settingType;
}

public static class SettingFiles
{
    public static FileOutputProfiles DefaultFileProfileSetting = new()
    {
        FileProfiles = new Dictionary<string, FileOutputProfile>
        {
            {
                "Default",
                 new FileOutputProfile
                 {
                    IsReadOnly=true,

                    EPGOutputProfile= new()
                    {

                    },
                        M3UOutputProfile= new()
                    {

                        EnableIcon=true,
                        TVGName=ValidM3USetting.Name.ToString(),
                        ChannelId=ValidM3USetting.Id.ToString(),
                        ChannelNumber=ValidM3USetting.ChannelNumber.ToString(),
                        TVGId=ValidM3USetting.EPGID.ToString(),
                        TVGGroup=ValidM3USetting.Group.ToString(),
                        GroupTitle=ValidM3USetting.Group.ToString(),

                    }
                 }
            },

             {
                "PLEX",
                 new FileOutputProfile
                 {
                    EPGOutputProfile= new()
                    {

                    },
                        M3UOutputProfile= new()
                    {

                        EnableIcon=true,
                        TVGName=ValidM3USetting.Name.ToString(),
                        ChannelId=ValidM3USetting.Id.ToString(),
                        ChannelNumber=ValidM3USetting.ChannelNumber.ToString(),
                        TVGId=ValidM3USetting.EPGID.ToString(),
                        TVGGroup=ValidM3USetting.Group.ToString(),
                        GroupTitle=ValidM3USetting.Group.ToString(),

                    }
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
                     Parameters = "-i {streamUrl} -c copy -f mpegts pipe:1"
                 }
             },
            {
                 "MP4",
                 new VideoOutputProfile
                 { IsReadOnly=true,
                     Command="ffmpeg",
                     Parameters = "-i {streamUrl} -c copy -f mp4 pipe:1"
                 }
             }
        }
    };
}

