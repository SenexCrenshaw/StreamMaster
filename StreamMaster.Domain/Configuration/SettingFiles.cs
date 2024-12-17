using StreamMaster.Domain.Crypto;

namespace StreamMaster.Domain.Configuration;

public static class SettingFiles
{
    public static readonly CustomLogoDict DefaultCustomLogoSetting = new()
    {
        CustomLogos = new Dictionary<string, CustomLogo>
        {
             {
                "/images/default.png".ToUrlSafeBase64String(),
                    new CustomLogo
                    {
                        IsReadOnly = true,
                        Name = "Default Logo",
                        Value= "/images/default.png".ToUrlSafeBase64String()
                    }
            },
              {
                "/images/streammaster_logo.png".ToUrlSafeBase64String(),
                    new CustomLogo
                    {
                        IsReadOnly = true,
                        Name = "Stream Master",
                        Value= "/images/streammaster_logo.png".ToUrlSafeBase64String()
                    }
            }
        }
    };

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
                //EnableId = true,
                EnableGroupTitle = true,
                EnableChannelNumber = true,
                Id = nameof(ValidM3USetting.ChannelNumber),
                Name = nameof(ValidM3USetting.Name),
                Group = nameof(ValidM3USetting.Group),
                //EPGId = nameof(ValidM3USetting.EPGId),
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
        },
            {
         "SMFFMPEG",
            new CommandProfile
            {
                IsReadOnly = true,
                Command = "ffmpeg",
                Parameters = "-hide_banner -loglevel error -user_agent {clientUserAgent} -fflags +genpts+discardcorrupt -i {streamUrl} -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts -copyts -reconnect 1 -reconnect_streamed 1 -reconnect_on_network_error 1 -reconnect_delay_max 10 -fflags +nobuffer pipe:1"
            }
            },

            {
         "SMFFMPEGLocal",
            new CommandProfile
            {
                IsReadOnly = true,
                Command = "ffmpeg",
                Parameters = "-hide_banner -loglevel error -re -i {streamUrl} -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1"
            }
            },

            {
         "YT",
            new CommandProfile
            {
                IsReadOnly = true,
                Command = "yt.sh",
                Parameters = "{streamUrl}"
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