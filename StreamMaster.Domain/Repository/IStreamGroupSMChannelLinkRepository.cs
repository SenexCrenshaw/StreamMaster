using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Repository;

public interface IStreamGroupSMChannelLinkRepository
{
    IQueryable<StreamGroupSMChannelLink> GetQuery(bool tracking = false);
    Task<APIResponse> AddSMChannelToStreamGroup(int StreamGroupId, int SMChannelId, bool? skipSave = false);
    Task AddSMChannelsToStreamGroup(int StreamGroupId, List<int> SMChannelIds);
    Task<APIResponse> RemoveSMChannelFromStreamGroup(int StreamGroupId, int SMChannelId);
    Task<APIResponse> RemoveSMChannelsFromStreamGroup(int StreamGroupId, List<int> SMChannelIds);
}