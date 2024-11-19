using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelStreamLinksRepository(ILogger<SMChannelStreamLinksRepository> intLogger, IRepositoryContext repositoryContext)
    : RepositoryBase<SMChannelStreamLink>(repositoryContext, intLogger), ISMChannelStreamLinksRepository
{
    public List<SMChannelStreamLink> GetSMChannelStreamLinks()
    {
        return [.. GetQuery()];
    }

    public async Task UpdateSMChannelDtoRanks(SMChannelDto smChannel)
    {
        List<SMChannelStreamLink> links = await GetQuery(true).Where(a => a.SMChannelId == smChannel.Id).ToListAsync();

        foreach (SMStreamDto stream in smChannel.SMStreamDtos)
        {
            SMChannelStreamLink? link = links.Find(a => a.SMStreamId == stream.Id);

            if (link != null)
            {
                stream.Rank = link.Rank;
            }
        }
    }
    public async Task CreateSMChannelStreamLink(int SMChannelId, string SMStreamId, int? Rank)
    {
        if (Any(a => a.SMStreamId == SMStreamId && a.SMChannelId == SMChannelId))
        {
            return;
        }
        SMChannel? smChannel = await RepositoryContext.SMChannels.FirstOrDefaultAsync(a => a.Id == SMChannelId);
        SMStream? smStream = await RepositoryContext.SMStreams.FirstOrDefaultAsync(a => a.Id == SMStreamId);
        if (smChannel != null && smStream != null)
        {
            int nextRank = Rank ?? GetMaxRank(smChannel.Id);

            SMChannelStreamLink link = new()
            {
                SMStream = smStream,
                SMChannel = smChannel,
                SMChannelId = smChannel.Id,
                SMStreamId = smStream.Id,
                Rank = nextRank,
            };


            RepositoryContext.SMChannelStreamLinks.Add(link);
            //SMChannel.SMStreams.Add(link);
            await SaveChangesAsync();
        }
    }

    public void CreateSMChannelStreamLink(SMChannel smChannel, string smStreamId, int? Rank)
    {
        if (Any(a => a.SMStreamId == smStreamId && a.SMChannelId == smChannel.Id))
        {
            return;
        }

        if (smChannel != null && smStreamId != null)
        {
            SMStream? smStream = RepositoryContext.SMStreams.FirstOrDefault(a => a.Id == smStreamId);
            int nextRank = Rank ?? GetMaxRank(smChannel.Id);

            SMChannelStreamLink link = new SMChannelStreamLink()
            {
                SMChannel = smChannel,
                SMChannelId = smChannel.Id,
                SMStreamId = smStreamId,
                Rank = nextRank,
            };

            RepositoryContext.SMChannelStreamLinks.Add(link);

        }
    }
    public void CreateSMChannelStreamLink(SMChannel smChannel, SMStream smStream, int? Rank)
    {
        if (Any(a => a.SMStreamId == smStream.Id && a.SMChannelId == smChannel.Id))
        {
            return;
        }

        if (smChannel != null && smStream != null)
        {
            int nextRank = Rank ?? GetMaxRank(smChannel.Id);

            SMChannelStreamLink link = new()
            {
                SMStream = smStream,
                SMChannel = smChannel,
                SMChannelId = smChannel.Id,
                SMStreamId = smStream.Id,
                Rank = nextRank,
            };

            RepositoryContext.SMChannelStreamLinks.Add(link);

        }
    }

    private int GetMaxRank(int SMChannelId)
    {
        List<SMChannelStreamLink> links = [.. GetQuery(false).Where(a => a.SMChannelId == SMChannelId)];
        if (links.Count == 0)
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

    public async Task DeleteSMChannelStreamLinksFromParentId(int smChannelId)
    {
        IQueryable<SMChannelStreamLink> linksToDelete = GetQuery(false).Where(a => a.SMChannelId == smChannelId);
        if (linksToDelete.Any())
        {
            await linksToDelete.ExecuteDeleteAsync().ConfigureAwait(false);
        }
    }

    public async Task DeleteSMChannelStreamLinks(IQueryable<SMChannelStreamLink> linksToDelete)
    {
        if (linksToDelete.Any())
        {
            List<int> smChannelIds = [.. linksToDelete.Select(a => a.SMChannelId)];
            foreach (SMChannelStreamLink? link in linksToDelete)
            {
                Delete(link);
            }
            await SaveChangesAsync();
            await UpdateRanks(smChannelIds);
        }
    }

    public async Task<APIResponse> SetSMStreamRank(List<SMChannelStreamRankRequest> request)
    {
        foreach (SMChannelStreamRankRequest r in request)
        {
            SMChannelStreamLink? streamRank = await FirstOrDefaultAsync(a => a.SMChannelId == r.SMChannelId && a.SMStreamId == r.SMStreamId, tracking: true);
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
            foreach (SMChannelStreamLink? link in GetQuery(true).Where(a => a.SMChannelId == smchannelId).OrderBy(a => a.Rank))
            {
                link.Rank = index++;
                Update(link);
            }
            await SaveChangesAsync();
        }
    }
}
