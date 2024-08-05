namespace StreamMaster.Domain.Configuration;

public static class SettingFiles
{
    public static readonly OutputProfileDict DefaultOutputProfileSetting = new()
    {

        OutputProfiles = new Dictionary<string, OutputProfile>
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

    public static readonly CommandProfileDict DefaultCommandProfileSetting = new()
    {
        CommandProfiles = new Dictionary<string, CommandProfile>
    {

        {
            "Default",
            new CommandProfile
            {
                Command="STREAMMASTER",
                IsReadOnly = true
            }
        },{
         "SMFFMPEG",
            new CommandProfile
            {
                IsReadOnly = true,
                Command = "ffmpeg",
                Parameters = "-map 0:v -map 0:a? -map 0:s? -c copy"
            }
            },
        {
         "mkvToTs",
            new CommandProfile{
                IsReadOnly = true,
                Command = "ffmpeg",
                Parameters = "-map 0:v -map 0:a -map 0:a? -map 0:s? -c:v copy -c:a:0 copy -c:a:1 ac3 -c:s copy"
            }
            },

        {
            "Redirect",
            new CommandProfile
            {
                IsReadOnly = true
            }
        }
    }
    };
}
