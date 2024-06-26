using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Repository
{
    public interface IStreamGroupSMChannelLinkRepository
    {
        IQueryable<StreamGroupSMChannelLink> GetQuery(bool tracking = false);
        Task CreateStreamGroupSMChannelLink(int StreamGroupId, int SMChannelId);
        Task<APIResponse> AddSMChannelToStreamGroup(int StreamGroupId, int SMChannelId);
        Task<APIResponse> RemoveSMChannelFromStreamGroup(int StreamGroupId, int SMChannelId);
    }
}