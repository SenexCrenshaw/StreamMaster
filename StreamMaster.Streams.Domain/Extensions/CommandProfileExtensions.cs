using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Domain.Extensions;

public static class CommandProfileExtensions
{
    public static CommandProfileDto ToCommandProfileDto(this CommandProfile commandProfile, string ProfileName)
    {
        return new CommandProfileDto
        {
            Command = commandProfile.Command,
            ProfileName = ProfileName,
            IsReadOnly = commandProfile.IsReadOnly,
            Parameters = commandProfile.Parameters,
            //Timeout = commandProfile.Timeout,
            //IsM3U8 = videoOutcommandProfileputProfile.IsM3U8
        };
    }
}
