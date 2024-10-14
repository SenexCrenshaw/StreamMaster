namespace StreamMaster.Domain.Configuration;

public interface IProfileDict<TProfile>
{
    Dictionary<string, TProfile> Profiles { get; }
}

public static class SettingFiles
{
    public static readonly OutputProfileDict DefaultOutputProfileSetting = new()
    {

        Profiles = new Dictionary<string, OutputProfile>
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
        Profiles = new Dictionary<string, CommandProfile>
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
                Parameters = "-hide_banner -loglevel error -user_agent {clientUserAgent} -fflags +genpts+discardcorrupt -thread_queue_size 1024 -reconnect_on_network_error 1 -reconnect_on_http_error 5xx,4xx -reconnect_streamed 1 -reconnect_delay_max 2 -reconnect 1 -i {streamUrl} -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts -muxdelay 0.001 -max_interleave_delta 0 -copyts pipe:1"
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
