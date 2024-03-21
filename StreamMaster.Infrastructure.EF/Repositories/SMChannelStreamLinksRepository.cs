using AutoMapper;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelStreamLinksRepository(ILogger<SMChannelStreamLinksRepository> intLogger, IRepositoryContext repositoryContext, IMapper mapper)
    : RepositoryBase<SMChannelStreamLink>(repositoryContext, intLogger), ISMChannelStreamLinksRepository
{
    public List<SMChannelStreamLink> GetSMChannelStreamLinks()
    {
        return [.. GetQuery()];
    }

    public async Task CreateSMChannelStreamLink(int SMChannelId, string SMStreamId)
    {

        if (Any(a => a.SMStreamId == SMStreamId && a.SMChannelId == SMChannelId))
        {
            return;
        }

        int nextRank = GetMaxRank(SMChannelId);
        SMChannelStreamLink link = new()
        {
            SMChannelId = SMChannelId,
            SMStreamId = SMStreamId,
            Rank = nextRank
        };

        Create(link);
        await SaveChangesAsync();
    }

    private int GetMaxRank(int SMChannelId)
    {
        List<SMChannelStreamLink> links = GetQuery(false).Where(a => a.SMChannelId == SMChannelId).ToList();
        if (!links.Any())
        {
            return 0;
        }
        int nextRank = links.Max(a => a.Rank) + 1;
        return nextRank;
    }

    public async Task DeleteSMChannelStreamLink(SMChannelStreamLink sMChannelStreamLink)
    {
        Delete(sMChannelStreamLink);
        await SaveChangesAsync();
    }

    public async Task DeleteSMChannelStreamLinksFromParentId(int smchannelId)
    {
        IQueryable<SMChannelStreamLink> linksToDelete = GetQuery(true).Where(a => a.SMChannelId == smchannelId);
        if (linksToDelete.Any())
        {
            foreach (SMChannelStreamLink? link in linksToDelete)
            {
                Delete(link);
            }
            await SaveChangesAsync();
        }
        List<int> smchannelIds = linksToDelete.Select(a => a.SMChannelId).ToList();
        await UpdateRanks(smchannelIds);
    }

    public async Task DeleteSMChannelStreamLinks(IQueryable<SMChannelStreamLink> linksToDelete)
    {
        if (linksToDelete.Any())
        {
            List<int> smchannelIds = linksToDelete.Select(a => a.SMChannelId).ToList();
            foreach (SMChannelStreamLink? link in linksToDelete)
            {
                Delete(link);
            }
            await SaveChangesAsync();
            await UpdateRanks(smchannelIds);
        }
    }

    public async Task<DefaultAPIResponse> SetSMStreamRank(List<SMChannelRankRequest> request)
    {
        foreach (SMChannelRankRequest r in request)
        {
            SMChannelStreamLink? streamRank = await GetQuery(true).FirstOrDefaultAsync(a => a.SMChannelId == r.SMChannelId && a.SMStreamId == r.SMStreamId);
            if (streamRank == null)
            {
                continue;
            }
            streamRank.Rank = r.Rank;
            Update(streamRank);
        }
        await SaveChangesAsync();
        return APIResponseFactory.Ok;

    }

    private async Task UpdateRanks(List<int> smchannelIds)
    {

        foreach (int smchannelId in smchannelIds)
        {
            int index = 0;
            foreach (SMChannelStreamLink? link in GetQuery(true).Where(a => a.SMChannelId == smchannelId).OrderBy(a => a.Rank))
            {
                link.Rank = index++;
                Update(link);
            }
            await SaveChangesAsync();
        }
    }
}
