namespace StreamMaster.Domain.Configuration;

public static class SettingFiles
{
    public static readonly OutputProfiles DefaultOutputProfileSetting = new()
    {
        OutProfiles = new Dictionary<string, OutputProfile>
    {
        {
            "Default",
            new OutputProfile
            {
                IsReadOnly = true,

                EnableIcon = true,
                EnableId = true,
                EnableGroupTitle = true,
                EnableChannelNumber = true,

                Name = nameof(ValidM3USetting.Name),
                Group = nameof(ValidM3USetting.Group),
                EPGId = nameof(ValidM3USetting.EPGId),
            }
        }
    }
    };

    public static readonly CommandProfileList DefauCommandProfileSetting = new()
    {
        CommandProfiles = new Dictionary<string, CommandProfile>
    {
        {
            "FFMPEG",
            new CommandProfile
            {
                IsReadOnly = true,
                Command = "ffmpeg",
                Parameters = "-hide_banner -loglevel error -user_agent {clientUserAgent} -i {streamUrl} -reconnect 1 -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1"
            }
        },
        {
            "StreamMaster",
            new CommandProfile
            {
                IsReadOnly = true
            }
        },
        {
            "None",
            new CommandProfile
            {
                IsReadOnly = true
            }
        },
        {
            BuildInfo.DefaultCommandProfileName,
            new CommandProfile
            {
                IsReadOnly = true
            }
        }
    }
    };
}
