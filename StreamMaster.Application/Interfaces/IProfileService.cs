namespace StreamMaster.Application.Interfaces
{
    public interface IProfileService
    {
        CommandProfileDto GetCommandProfile(string? CommandProfileName = null);
        List<CommandProfileDto> GetCommandProfiles();
    }
}