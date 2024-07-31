namespace StreamMaster.Application.Profiles;

public class ProfileService(IOptionsMonitor<Setting> intSettings, IOptionsMonitor<OutputProfiles> intOutProfileSettings, IOptionsMonitor<CommandProfiles> intCommandProfileSettings
    ) : IProfileService
{
    public List<CommandProfileDto> GetCommandProfiles()
    {
        return intCommandProfileSettings.CurrentValue.GetProfilesDto();
    }

    public CommandProfileDto GetCommandProfile(string? CommandProfileName = null)
    {
        Setting settings = intSettings.CurrentValue;

        return CommandProfileName is not null && CommandProfileName != settings.DefaultCommandProfileName
            ? intCommandProfileSettings.CurrentValue.GetProfileDto(CommandProfileName)
            : intCommandProfileSettings.CurrentValue.GetDefaultProfileDto(settings.DefaultCommandProfileName);
    }

    public OutputProfileDto GetOutputProfile(string? OutputProfileName = null)
    {
        Setting settings = intSettings.CurrentValue;

        return OutputProfileName is not null && OutputProfileName != settings.DefaultOutputProfileName
           ? intOutProfileSettings.CurrentValue.GetProfileDto(OutputProfileName)
           : intOutProfileSettings.CurrentValue.GetDefaultProfileDto(settings.DefaultOutputProfileName);

        //Setting settings = intSettings.CurrentValue;

        //string profileName = settings.DefaultOutputProfileName;

        //if (OutputProfileName is not null && OutputProfileName != settings.DefaultOutputProfileName)
        //{
        //    profileName = OutputProfileName;
        //}

        //if (intOutProfileSettings.CurrentValue.Profiles.TryGetValue(profileName, out OutputProfile? profile))
        //{
        //    OutputProfileDto ret = new()
        //    {
        //        ProfileName = profileName,
        //        IsReadOnly = profile.IsReadOnly,
        //        EnableIcon = profile.EnableIcon,
        //        EnableId = profile.EnableId,
        //        EnableGroupTitle = profile.EnableGroupTitle,
        //        EnableChannelNumber = profile.EnableChannelNumber,
        //        Name = profile.Name,
        //        EPGId = profile.EPGId,
        //        Group = profile.Group,
        //    };
        //    return ret;
        //}

        //return null;
    }
}
