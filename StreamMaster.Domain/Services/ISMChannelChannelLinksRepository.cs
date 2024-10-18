using StreamMaster.Domain.API;
using StreamMaster.Domain.Repository;

namespace StreamMaster.Domain.Services
{
    public interface ISMChannelChannelLinksRepository : IRepositoryBase<SMChannelChannelLink>
    {
        Task CreateSMChannelChannelLink(int ParentSMChannelId, int ChildSMChannelId, int? Rank);
        void CreateSMChannelChannelLink(SMChannel smChannel, int ChildSMChannelId, int? Rank);
        void CreateSMChannelChannelLink(SMChannel smChannel, SMChannel ChildSMChannel, int? Rank);
        Task DeleteSMChannelChannelLink(SMChannelChannelLink SMChannelChannelLink);
        Task DeleteSMChannelChannelLinks(IQueryable<SMChannelChannelLink> linksToDelete);
        Task DeleteSMChannelChannelLinksFromParentId(int smChannelId);
        List<SMChannelChannelLink> GetSMChannelChannelLinks();
        Task<APIResponse> SetSMChannelRanks(List<SMChannelChannelRankRequest> request);
        Task UpdateSMChannelDtoRanks(SMChannelDto smChannel);
    }
}