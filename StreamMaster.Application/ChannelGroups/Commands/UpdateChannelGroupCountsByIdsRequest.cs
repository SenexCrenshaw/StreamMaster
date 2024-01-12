using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace StreamMaster.Application.ChannelGroups.Commands;

public record UpdateChannelGroupCountsByIdsRequest(List<int>? ChannelGroupIds = null) : IRequest<List<ChannelGroupDto>> { }

[LogExecutionTimeAspect]
public class UpdateChannelGroupCountsByIdsRequestHandler(ILogger<UpdateChannelGroupCountsByIdsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IMemoryCache memoryCache) : IRequestHandler<UpdateChannelGroupCountsByIdsRequest, List<ChannelGroupDto>>
{
    private class ChannelGroupBrief
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public async Task<List<ChannelGroupDto>> Handle(UpdateChannelGroupCountsByIdsRequest request, CancellationToken cancellationToken)
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

            cgs = await repository.ChannelGroup.GetChannelGroups(request.ChannelGroupIds);

            if (!cgs.Any())
            {
                logger.LogInformation("No channel groups found based on the request.");
                return [];
            }

            List<ChannelGroupDto> dtos = mapper.Map<List<ChannelGroupDto>>(cgs);

            List<string> cgNames = dtos.Select(a => a.Name).ToList();

            // Fetch relevant video streams.
            var allVideoStreams = await repository.VideoStream.GetVideoStreamQuery()
                .Where(a => cgNames.Contains(a.User_Tvg_group))
                .Select(vs => new
                {
                    vs.Id,
                    vs.User_Tvg_group,
                    vs.IsHidden
                }).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            Dictionary<string, List<string>> videoStreamsForGroups = [];
            Dictionary<string, int> hiddenCounts = [];
            ChannelGroupDto? c = dtos.FirstOrDefault(a => a.Id == 29);

            foreach (ChannelGroupDto cg in dtos)
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
                foreach (ChannelGroupDto cg in dtos)
                {
                    cg.TotalCount = videoStreamsForGroups[cg.Name].Count;
                    cg.ActiveCount = videoStreamsForGroups[cg.Name].Count - hiddenCounts[cg.Name];
                    cg.HiddenCount = hiddenCounts[cg.Name];
                    cg.IsHidden = hiddenCounts[cg.Name] != 0;
                }

                memoryCache.AddOrUpdateChannelGroupVideoStreamCounts(dtos);
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
            logger.LogError(ex, "Error while handling UpdateChannelGroupCountsByIdsRequest.");
            throw; // Re-throw the exception if needed or handle accordingly.
        }
        return [];
    }
}