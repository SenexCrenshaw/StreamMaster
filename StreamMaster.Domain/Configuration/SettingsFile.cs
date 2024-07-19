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

                        Name=nameof(ValidM3USetting.Name),
                        Group=nameof(ValidM3USetting.Group),
                        EPGId=nameof(ValidM3USetting.EPGId),
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
                "FFMPEG",
                new VideoOutputProfile
                {
                    IsReadOnly=true,
                    Command="ffmpeg",
                    Parameters = "-hide_banner -loglevel error -user_agent {clientUserAgent} -i {streamUrl} -reconnect 1 -map 0:v -map 0:a? -map 0:s? -c copy -bsf:v h264_mp4toannexb -f mpegts pipe:1"
                }
            },
             {
                "StreamMaster",
                new VideoOutputProfile
                {
                    IsReadOnly=true
                }
            },
             {
                "None",
                new VideoOutputProfile
                {
                    IsReadOnly=true
                }
            }
        }
    };
}

