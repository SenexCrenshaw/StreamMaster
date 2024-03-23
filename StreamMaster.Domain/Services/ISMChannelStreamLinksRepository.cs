﻿using StreamMaster.Domain.API;
using StreamMaster.Domain.Repository;

namespace StreamMaster.Domain.Services
{
    public interface ISMChannelStreamLinksRepository : IRepositoryBase<SMChannelStreamLink>
    {
        IQueryable<SMChannelStreamLink> GetQuery(bool tracking = false);

        Task CreateSMChannelStreamLink(int SMChannelId, string SMStreamId);
        Task DeleteSMChannelStreamLink(SMChannelStreamLink sMChannelStreamLink);
        Task DeleteSMChannelStreamLinks(IQueryable<SMChannelStreamLink> linksToDelete);
        Task DeleteSMChannelStreamLinksFromParentId(int smchannelId);

        List<SMChannelStreamLink> GetSMChannelStreamLinks();
        Task<DefaultAPIResponse> SetSMStreamRank(List<SMChannelRankRequest> request);
    }
}