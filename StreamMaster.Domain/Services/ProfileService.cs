using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Repository;

namespace StreamMaster.Domain.Services;

public class ProfileService(IOptionsMonitor<Setting> intSettings, IServiceProvider serviceProvider, IOptionsMonitor<OutputProfileDict> intOutProfileSettings, IOptionsMonitor<CommandProfileDict> intCommandProfileSettings
    ) : IProfileService
{
    public List<CommandProfileDto> GetCommandProfiles()
    {
        return intCommandProfileSettings.CurrentValue.GetProfilesDto();
    }

    public CommandProfileDto GetCommandProfile(string? CommandProfileName = null)
    {
        Setting settings = intSettings.CurrentValue;

        return !string.IsNullOrEmpty(CommandProfileName) && CommandProfileName != settings.DefaultCommandProfileName
            ? intCommandProfileSettings.CurrentValue.GetProfileDto(CommandProfileName)
            : intCommandProfileSettings.CurrentValue.GetDefaultProfileDto(settings.DefaultCommandProfileName);
    }

    public OutputProfileDto GetOutputProfile(string? OutputProfileName = null)
    {
        Setting settings = intSettings.CurrentValue;

        return !string.IsNullOrEmpty(OutputProfileName) && OutputProfileName != settings.DefaultOutputProfileName
           ? intOutProfileSettings.CurrentValue.GetProfileDto(OutputProfileName)
           : intOutProfileSettings.CurrentValue.GetDefaultProfileDto(settings.DefaultOutputProfileName);
    }

    public CommandProfileDto GetM3U8OutputProfile(string id)
    {
        Setting settings = intSettings.CurrentValue;
        using IServiceScope scope = serviceProvider.CreateScope();
        IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
        SMStream? smStream = repositoryWrapper.SMStream.GetSMStreamById(id);
        if (smStream?.M3UFileId > 0)
        {
            M3UFile? m3uFile = repositoryWrapper.M3UFile.GetQuery().FirstOrDefault(m => m.Id == smStream.M3UFileId);
            if (m3uFile != null && !string.IsNullOrEmpty(m3uFile.M3U8OutPutProfile) && intCommandProfileSettings.CurrentValue.HasProfile(m3uFile.M3U8OutPutProfile))
            {
                return intCommandProfileSettings.CurrentValue.GetProfileDto(m3uFile.M3U8OutPutProfile);
            }
        }

        if (!string.IsNullOrEmpty(settings.M3U8OutPutProfile))
        {
            if (intCommandProfileSettings.CurrentValue.HasProfile(settings.M3U8OutPutProfile))
            {
                CommandProfileDto ret = intCommandProfileSettings.CurrentValue.GetProfileDto(settings.M3U8OutPutProfile);
                return ret;
            }
        }
        return intCommandProfileSettings.CurrentValue.GetProfileDto("SMFFMPEG");
    }
}