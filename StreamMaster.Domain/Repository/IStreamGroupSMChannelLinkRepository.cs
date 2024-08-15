using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Repository;

public interface IStreamGroupSMChannelLinkRepository : IRepositoryBase<StreamGroupSMChannelLink>
{
    new IQueryable<StreamGroupSMChannelLink> GetQuery(bool tracking = false);
    Task<APIResponse> AddSMChannelToStreamGroup(int StreamGroupId, int SMChannelId, bool? skipSave = false, bool? skipCheck = false);
    Task AddSMChannelsToStreamGroupAsync(int StreamGroupId, List<int> SMChannelIds, bool? skipSave = false);
    Task<APIResponse> RemoveSMChannelFromStreamGroup(int StreamGroupId, int SMChannelId);
    Task<APIResponse> RemoveSMChannelsFromStreamGroup(int StreamGroupId, List<int> SMChannelIds);
}