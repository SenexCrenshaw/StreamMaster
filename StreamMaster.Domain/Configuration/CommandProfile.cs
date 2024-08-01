namespace StreamMaster.Domain.Configuration;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class CommandProfile
{
    public bool IsReadOnly { get; set; } = false;
    public string Command { get; set; } = "ffmpeg";
    public string Parameters { get; set; } = "";
    //public int Timeout { get; set; } = 20;
    //public bool IsM3U8 { get; set; } = false;

}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class CommandProfileDto : CommandProfile
{
    public string ProfileName { get; set; } = "";
}

public class CommandProfiles
{
    public Dictionary<string, CommandProfile> Profiles { get; set; } = [];
    public CommandProfile? GetProfile(string CommandProfileName)
    {
        return Profiles.TryGetValue(CommandProfileName, out CommandProfile? existingProfile)
            ? existingProfile
            : null;
    }

    public bool HasProfile(string CommandProfileName)
    {
        return Profiles.TryGetValue(CommandProfileName, out _);
    }

    public CommandProfileDto GetProfileDto(string CommandProfileName)
    {
        return GetDefaultProfileDto(CommandProfileName);
    }

    public CommandProfileDto GetDefaultProfileDto(string defaultName = "Default")
    {

        CommandProfile? defaultProfile = GetProfile(defaultName);
        return defaultProfile == null
            ? throw new Exception($"Command Profile {defaultName} not found")
            : GetProfileDtoFromProfile(defaultProfile, defaultName);
    }

    public CommandProfileDto GetProfileDtoFromProfile(CommandProfile commandProfile, string ProfileName)
    {
        return new CommandProfileDto
        {
            Command = commandProfile.Command,
            ProfileName = ProfileName,
            IsReadOnly = commandProfile.IsReadOnly,
            Parameters = commandProfile.Parameters,
        };
    }

    public List<CommandProfileDto> GetProfilesDto()
    {
        List<CommandProfileDto> ret = [];

        foreach (string key in Profiles.Keys)
        {
            if (Profiles.TryGetValue(key, out CommandProfile? profile))
            {
                ret.Add(GetProfileDtoFromProfile(profile, key));
            }
        }
        return ret;
    }

    public List<CommandProfile> GetProfiles()
    {
        List<CommandProfile> ret = [];

        foreach (string key in Profiles.Keys)
        {
            if (Profiles.TryGetValue(key, out CommandProfile? profile))
            {
                ret.Add(profile);
            }
        }
        return ret;
    }
}