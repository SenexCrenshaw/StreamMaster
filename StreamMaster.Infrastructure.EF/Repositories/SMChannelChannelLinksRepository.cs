using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelChannelLinksRepository(ILogger<SMChannelChannelLinksRepository> intLogger, IRepositoryContext repositoryContext)
    : RepositoryBase<SMChannelChannelLink>(repositoryContext, intLogger), ISMChannelChannelLinksRepository
{
    public List<SMChannelChannelLink> GetSMChannelChannelLinks()
    {
        return [.. GetQuery()];
    }

    public async Task CreateSMChannelChannelLink(int ParentSMChannelId, int ChildSMChannelId, int? Rank)
    {
        if (Any(a => a.ParentSMChannelId == ParentSMChannelId && a.SMChannelId == ChildSMChannelId))
        {
            return;
        }
        SMChannel? parentSMChannel = await RepositoryContext.SMChannels.FirstOrDefaultAsync(a => a.Id == ParentSMChannelId);
        SMChannel? childSMChannel = await RepositoryContext.SMChannels.FirstOrDefaultAsync(a => a.Id == ChildSMChannelId);

        if (parentSMChannel != null && childSMChannel != null)
        {
            int nextRank = Rank ?? GetMaxRank(parentSMChannel.Id);

            SMChannelChannelLink link = new()
            {
                ParentSMChannel = parentSMChannel,
                ParentSMChannelId = parentSMChannel.Id,
                SMChannel = childSMChannel,
                SMChannelId = childSMChannel.Id,
                Rank = nextRank,
            };
            RepositoryContext.SMChannelChannelLinks.Add(link);
            await SaveChangesAsync();
        }
    }

    public void CreateSMChannelChannelLink(SMChannel smChannel, int ChildSMChannelId, int? Rank)
    {
        if (Any(a => a.ParentSMChannelId == smChannel.Id && a.SMChannelId == ChildSMChannelId))
        {
            return;
        }

        if (smChannel != null)
        {
            int nextRank = Rank ?? GetMaxRank(smChannel.Id);

            SMChannelChannelLink link = new()
            {
                ParentSMChannel = smChannel,
                ParentSMChannelId = smChannel.Id,
                SMChannelId = ChildSMChannelId,
                Rank = nextRank,
            };

            RepositoryContext.SMChannelChannelLinks.Add(link);
        }
    }
    public void CreateSMChannelChannelLink(SMChannel smChannel, SMChannel ChildSMChannel, int? Rank)
    {
        if (Any(a => a.ParentSMChannelId == smChannel.Id && a.SMChannelId == ChildSMChannel.Id))
        {
            return;
        }

        if (smChannel != null)
        {
            int nextRank = Rank ?? GetMaxRank(smChannel.Id);

            SMChannelChannelLink link = new()
            {
                ParentSMChannel = smChannel,
                ParentSMChannelId = smChannel.Id,
                SMChannel = ChildSMChannel,
                SMChannelId = ChildSMChannel.Id,
                Rank = nextRank,
            };

            RepositoryContext.SMChannelChannelLinks.Add(link);
        }
    }

    private int GetMaxRank(int SMChannelId)
    {
        List<SMChannelChannelLink> links = [.. GetQuery(false).Where(a => a.SMChannelId == SMChannelId)];
        if (links.Count == 0)
        {
            return 0;
        }
        int nextRank = links.Max(a => a.Rank) + 1;
        return nextRank;
    }

    public async Task DeleteSMChannelChannelLink(SMChannelChannelLink SMChannelChannelLink)
    {
        Delete(SMChannelChannelLink);
        await SaveChangesAsync();
    }

    public async Task DeleteSMChannelChannelLinksFromParentId(int smChannelId)
    {
        IQueryable<SMChannelChannelLink> linksToDelete = GetQuery(false).Where(a => a.SMChannelId == smChannelId);
        if (linksToDelete.Any())
        {
            await linksToDelete.ExecuteDeleteAsync().ConfigureAwait(false);
        }
    }

    public async Task DeleteSMChannelChannelLinks(IQueryable<SMChannelChannelLink> linksToDelete)
    {
        if (linksToDelete.Any())
        {
            List<int> smChannelIds = [.. linksToDelete.Select(a => a.SMChannelId)];
            foreach (SMChannelChannelLink? link in linksToDelete)
            {
                Delete(link);
            }
            await SaveChangesAsync();
            await UpdateRanks(smChannelIds);
        }
    }

    public async Task<APIResponse> SetSMChannelRanks(List<SMChannelChannelRankRequest> request)
    {
        foreach (SMChannelChannelRankRequest r in request)
        {
            SMChannelChannelLink? streamRank = await FirstOrDefaultAsync(a => a.ParentSMChannelId == r.ParentSMChannelId && a.SMChannelId == r.ChildSMChannelId, tracking: true);
            if (streamRank == null)
            {
                continue;
            }
            streamRank.Rank = r.Rank;
            Update(streamRank);
        }
        await SaveChangesAsync();
        return APIResponse.Success;
    }

    private async Task UpdateRanks(List<int> smchannelIds)
    {
        foreach (int smchannelId in smchannelIds)
        {
            int index = 0;
            foreach (SMChannelChannelLink? link in GetQuery(true).Where(a => a.SMChannelId == smchannelId).OrderBy(a => a.Rank))
            {
                link.Rank = index++;
                Update(link);
            }
            await SaveChangesAsync();
        }
    }
}
