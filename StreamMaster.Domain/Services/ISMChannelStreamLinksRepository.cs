using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Services
{
    public interface ISMChannelStreamLinksRepository
    {
        Task CreateSMChannelStreamLink(int SMChannelId, string SMStreamId);
        Task DeleteSMChannelStreamLink(SMChannelStreamLink sMChannelStreamLink);
        Task DeleteSMChannelStreamLinks(IQueryable<SMChannelStreamLink> linksToDelete);
        Task DeleteSMChannelStreamLinksFromParentId(int smchannelId);
        IQueryable<SMChannelStreamLink> GetQuery(bool tracking = false);
        List<SMChannelStreamLink> GetSMChannelStreamLinks();
        Task<DefaultAPIResponse> SetSMStreamRank(List<SMChannelRankRequest> request);
        void Update(SMChannelStreamLink streamRank);
    }
}