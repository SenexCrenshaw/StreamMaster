namespace StreamMaster.Domain.Services
{
    public interface ISMChannelStreamLinksRepository
    {
        Task CreateSMChannelStreamLink(SMChannelStreamLink sMChannelStreamLink);
        Task DeleteSMChannelStreamLink(SMChannelStreamLink sMChannelStreamLink);
        Task DeleteSMChannelStreamLinks(IQueryable<SMChannelStreamLink> linksToDelete);
        Task DeleteSMChannelStreamLinksFromParentId(int smchannelId);
        IQueryable<SMChannelStreamLink> GetQuery(bool tracking = false);
        List<SMChannelStreamLink> GetSMChannelStreamLinks();
    }
}