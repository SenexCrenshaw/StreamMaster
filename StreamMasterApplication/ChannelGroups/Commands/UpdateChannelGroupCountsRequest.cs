using FluentValidation;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record UpdateChannelGroupCountsRequest(IEnumerable<ChannelGroupDto>? channelGroups = null) : IRequest<List<ChannelGroupDto>> { }

[LogExecutionTimeAspect]
public class UpdateChannelGroupCountsRequestHandler(ILogger<UpdateChannelGroupCountsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupCountsRequest, List<ChannelGroupDto>>
{
    private class ChannelGroupBrief
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public async Task<List<ChannelGroupDto>> Handle(UpdateChannelGroupCountsRequest request, CancellationToken cancellationToken)
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
                return new();
            }

            var dtos = mapper.Map<List<ChannelGroupDto>>(cgs);

            List<string> cgNames = dtos.Select(a => a.Name).ToList();

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
            var c = dtos.FirstOrDefault(a => a.Id == 29);

            foreach (var cg in dtos)
            {
                if (cg == null)
                {
                    continue;
                }

                var relevantStreams = allVideoStreams.Where(vs => vs.User_Tvg_group == cg.Name).ToList();

                videoStreamsForGroups[cg.Name] = relevantStreams.Select(vs => vs.Id).ToList();
                hiddenCounts[cg.Name] = relevantStreams.Count(vs => vs.IsHidden);
            }

            if (dtos.Any())
            {
                foreach (var cg in dtos)
                {
                    cg.TotalCount = videoStreamsForGroups[cg.Name].Count;
                    cg.ActiveCount = videoStreamsForGroups[cg.Name].Count - hiddenCounts[cg.Name];
                    cg.HiddenCount = hiddenCounts[cg.Name];
                    cg.IsHidden = hiddenCounts[cg.Name] != 0;
                }

                MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(dtos);
                return dtos;
                //await HubContext.Clients.All.ChannelGroupsRefresh(dtos.ToArray()).ConfigureAwait(false);

                //if (channelGroupStreamCounts.Any())
                //{
                //    var a = channelGroupStreamCounts.FirstOrDefault(a => a.ChannelGroupId == 29);
                //    MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts);
                //    var b = channelGroupStreamCounts.FirstOrDefault(a => a.ChannelGroupId == 29);
                //    await HubContext.Clients.All.UpdateChannelGroups(cgs).ConfigureAwait(false);
                //}
            }

            //if (channelGroupStreamCounts.Any())
            //{
            //    MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts);
            //    await HubContext.Clients.All.UpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts).ConfigureAwait(false);
            //}
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while handling UpdateChannelGroupCountsRequest.");
            throw; // Re-throw the exception if needed or handle accordingly.
        }
        return new();
    }
}