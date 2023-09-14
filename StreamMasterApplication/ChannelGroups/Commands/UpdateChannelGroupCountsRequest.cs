using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record UpdateChannelGroupCountsRequest(IEnumerable<ChannelGroupDto>? channelGroups = null) : IRequest { }


[LogExecutionTimeAspect]
public class UpdateChannelGroupCountsRequestHandler(ILogger<UpdateChannelGroupCountsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupCountsRequest>
{
    private class ChannelGroupBrief
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public async Task Handle(UpdateChannelGroupCountsRequest request, CancellationToken cancellationToken)
    {
        // Get the required channel groups.
        IQueryable<ChannelGroupBrief> cgsQuery = Repository.ChannelGroup.GetChannelGroupQuery().Select(a => new ChannelGroupBrief { Name = a.Name, Id = a.Id });
        if (request.channelGroups != null)
        {
            List<int> selectIds = request.channelGroups.Select(a => a.Id).ToList();
            cgsQuery = cgsQuery.Where(a => selectIds.Contains(a.Id));
        }

        List<ChannelGroupBrief> cgs = await cgsQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
        List<string> cgNames = cgs.Select(a => a.Name).ToList();

        // Fetch relevant video streams.
        var allVideoStreams = await Repository.VideoStream.GetVideoStreamQuery()
            .Where(a => cgNames.Contains(a.User_Tvg_group))
            .Select(vs => new
            {
                vs.Id,
                vs.User_Tvg_group,
                vs.IsHidden
            }).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        Dictionary<string, List<string>> videoStreamsForGroups = new();
        Dictionary<string, int> hiddenCounts = new();

        foreach (ChannelGroupBrief? cg in cgs)
        {
            var relevantStreams = allVideoStreams.Where(vs => vs.User_Tvg_group == cg.Name).ToList();

            videoStreamsForGroups[cg.Name] = relevantStreams.Select(vs => vs.Id).ToList();
            hiddenCounts[cg.Name] = relevantStreams.Count(vs => vs.IsHidden);
        }

        List<ChannelGroupStreamCount> channelGroupStreamCounts = new();

        foreach (ChannelGroupBrief? cg in cgs)
        {
            ChannelGroupStreamCount response = new();

            List<string> ids = videoStreamsForGroups[cg.Name];
            int hiddenCount = hiddenCounts[cg.Name];

            response.TotalCount = ids.Count;
            response.ActiveCount = ids.Count - hiddenCount;
            response.HiddenCount = hiddenCount;
            response.ChannelGroupId = cg.Id;

            channelGroupStreamCounts.Add(response);
        }

        if (channelGroupStreamCounts.Any())
        {
            MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts);
            await HubContext.Clients.All.UpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts).ConfigureAwait(false);
        }
    }

}
