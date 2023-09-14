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
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            List<ChannelGroup> cgs;

            //// Get the required channel groups.
            //IQueryable<ChannelGroup> cgsQuery = Repository.ChannelGroup.GetChannelGroupQuery().Select(a => new ChannelGroupBrief { Name = a.Name, Id = a.Id });

            if (request.channelGroups != null && request.channelGroups.Any())
            {
                List<int> selectIds = request.channelGroups.Select(a => a.Id).ToList();
                cgs = await Repository.ChannelGroup.GetChannelGroups(selectIds);
            }
            else
            {
                cgs = await Repository.ChannelGroup.GetChannelGroups();
            }

            if (!cgs.Any())
            {
                Logger.LogInformation("No channel groups found based on the request.");
                return;
            }

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

            foreach (ChannelGroup cg in cgs)
            {
                if (cg == null)
                {
                    continue;
                }

                var relevantStreams = allVideoStreams.Where(vs => vs.User_Tvg_group == cg.Name).ToList();

                videoStreamsForGroups[cg.Name] = relevantStreams.Select(vs => vs.Id).ToList();
                hiddenCounts[cg.Name] = relevantStreams.Count(vs => vs.IsHidden);
            }

            List<ChannelGroupStreamCount> channelGroupStreamCounts = cgs.Where(cg => cg != null).Select(cg => new ChannelGroupStreamCount
            {
                TotalCount = videoStreamsForGroups[cg.Name].Count,
                ActiveCount = videoStreamsForGroups[cg.Name].Count - hiddenCounts[cg.Name],
                HiddenCount = hiddenCounts[cg.Name],
                ChannelGroupId = cg.Id
            }).ToList();

            if (channelGroupStreamCounts.Any())
            {
                MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts);
                await HubContext.Clients.All.UpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while handling UpdateChannelGroupCountsRequest.");
            throw; // Re-throw the exception if needed or handle accordingly.
        }
    }
}
