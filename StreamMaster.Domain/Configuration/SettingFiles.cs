namespace StreamMaster.Domain.Configuration;

public static class SettingFiles
{
    public static readonly OutputProfiles DefaultOutputProfileSetting = new()
    {
        Profiles = new Dictionary<string, OutputProfile>
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

    public static readonly CommandProfiles DefauCommandProfileSetting = new()
    {
        Profiles = new Dictionary<string, CommandProfile>
    {
        {
            "Default",
            new CommandProfile
            {
                IsReadOnly = true,
                Command = "ffmpeg",
                Parameters = "-map 0:v -map 0:a? -map 0:s? -c:v libx264 -c:a ac3"
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
        }
    }
    };
}
