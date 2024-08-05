namespace StreamMaster.Application.Profiles;

public class ProfileService(IOptionsMonitor<Setting> intSettings, IOptionsMonitor<OutputProfileDict> intOutProfileSettings, IOptionsMonitor<CommandProfileDict> intCommandProfileSettings
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
        ;
    }


}
