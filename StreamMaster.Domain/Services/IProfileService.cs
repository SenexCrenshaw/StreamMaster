using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Services
{
    public interface IProfileService
    {
        OutputProfileDto GetOutputProfile(string? OutputProfileName = null);
        CommandProfileDto GetCommandProfile(string? CommandProfileName = null);
        List<CommandProfileDto> GetCommandProfiles();
        CommandProfileDto GetM3U8OutputProfile(string id);
    }
}