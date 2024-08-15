namespace StreamMaster.Application.ChannelGroups;

public class ChannelGroupService(ILogger<ChannelGroupService> _logger, IRepositoryWrapper repositoryWrapper) : IChannelGroupService

{
    public async Task UpdateChannelGroupCountsRequestAsync(List<ChannelGroup>? ChannelGroups = null)
    {

        try
        {
            List<ChannelGroup> cgs = ChannelGroups == null || ChannelGroups.Count == 0
                ? [.. repositoryWrapper.ChannelGroup.GetQuery(true)]
                : ChannelGroups;

            if (cgs.Count != 0)
            {
                List<string> cgNames = cgs.ConvertAll(a => a.Name);

                List<SMStream> smStreams = [.. repositoryWrapper.SMStream.GetQuery().Where(a => cgNames.Contains(a.Group))];

                foreach (ChannelGroup cg in cgs)
                {
                    if (cg == null)
                    {
                        continue;
                    }

                    List<SMStream> relevantStreams = smStreams.Where(vs => vs.Group == cg.Name).ToList();

                    var counts = relevantStreams.GroupBy(vs => vs.IsHidden).Select(g => new { IsHidden = g.Key, Count = g.Count() }).ToList();
                    int totalCount = counts.Sum(c => c.Count);
                    int hiddenCount = counts.Find(c => c.IsHidden)?.Count ?? 0;
                    cg.TotalCount = totalCount;
                    cg.ActiveCount = totalCount - hiddenCount;
                    cg.HiddenCount = hiddenCount;
                }

                await repositoryWrapper.SaveAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling UpdateChannelGroupCountsRequest.");

            throw;
        }
    }

    public async Task<ChannelGroupDto> UpdateChannelGroupCountRequestAsync(ChannelGroupDto ChannelGroup)
    {
        try
        {
            IQueryable<SMStream> smStreams = repositoryWrapper.SMStream.GetQuery(true).Where(vs => vs.Group == ChannelGroup.Name);

            var counts = await smStreams.GroupBy(vs => vs.IsHidden).Select(g => new { IsHidden = g.Key, Count = g.Count() }).ToListAsync();
            int totalCount = counts.Sum(c => c.Count);
            int hiddenCount = counts.Find(c => c.IsHidden)?.Count ?? 0;

            ChannelGroup.TotalCount = totalCount;
            ChannelGroup.ActiveCount = totalCount - hiddenCount;
            ChannelGroup.HiddenCount = hiddenCount;
            await repositoryWrapper.SaveAsync();

            return ChannelGroup;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing UpdateChannelGroupCountRequest.");
            throw;
        }
    }

}
