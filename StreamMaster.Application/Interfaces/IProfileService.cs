namespace StreamMaster.Application.Interfaces
{
    public interface IProfileService
    {
        OutputProfileDto GetOutputProfile(string? OutputProfileName = null);
        CommandProfileDto GetCommandProfile(string? CommandProfileName = null);
        List<CommandProfileDto> GetCommandProfiles();
        CommandProfileDto GetM3U8OutputProfile(string id);
    }
}