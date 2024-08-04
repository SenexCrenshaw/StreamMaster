namespace StreamMaster.Application.Interfaces;

public interface IStreamGroupService
{
    Task<(List<VideoStreamConfig> videoStreamConfigs, StreamGroupProfile streamGroupProfile)> GetStreamGroupVideoConfigs(int StreamGroupId, int StreamGroupProfileId);
    Task<StreamGroup?> GetStreamGroupFromIdAsync(int streamGroupId);
    Task<StreamGroup?> GetStreamGroupFromNameAsync(string streamGroupName);
    Task<StreamGroup> GetDefaultSGAsync();
    Task<int> GetDefaultSGIdAsync();
    Task<StreamGroupProfile> GetDefaultStreamGroupProfileAsync();
    Task<CommandProfileDto> GetProfileFromSMChannelDtoAsync(int streamGroupId, int streamGroupProfileId, string CommandProfileName);
    Task<StreamGroupProfile> GetStreamGroupProfileAsync(int? StreamGroupId = null, int? StreamGroupProfileId = null);
}