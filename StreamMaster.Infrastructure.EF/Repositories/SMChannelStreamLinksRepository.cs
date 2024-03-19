using AutoMapper;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelStreamLinksRepository(ILogger<SMChannelStreamLinksRepository> intLogger, IRepositoryContext repositoryContext, IMapper mapper)
    : RepositoryBase<SMChannelStreamLink>(repositoryContext, intLogger), ISMChannelStreamLinksRepository
{
    public List<SMChannelStreamLink> GetSMChannelStreamLinks()
    {
        return [.. FindAll()];
    }

    public IQueryable<SMChannelStreamLink> GetQuery(bool tracking = false)
    {
        return tracking ? FindAllWithTracking() : FindAll();
    }

    public async Task CreateSMChannelStreamLink(SMChannelStreamLink sMChannelStreamLink)
    {
        Create(sMChannelStreamLink);
        await SaveChangesAsync();
    }
    public async Task DeleteSMChannelStreamLink(SMChannelStreamLink sMChannelStreamLink)
    {
        Delete(sMChannelStreamLink);
        await SaveChangesAsync();
    }

    public async Task DeleteSMChannelStreamLinksFromParentId(int smchannelId)
    {
        IQueryable<SMChannelStreamLink> links = GetQuery(true).Where(a => a.SMChannelId == smchannelId);
        if (links.Any())
        {
            foreach (SMChannelStreamLink? link in links)
            {
                Delete(link);
            }
            await SaveChangesAsync();
        }

    }

    public async Task DeleteSMChannelStreamLinks(IQueryable<SMChannelStreamLink> linksToDelete)
    {
        if (linksToDelete.Any())
        {
            foreach (SMChannelStreamLink? link in linksToDelete)
            {
                Delete(link);
            }
            await SaveChangesAsync();
        }
    }
}
