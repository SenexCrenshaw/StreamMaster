using FluentValidation;

using Microsoft.EntityFrameworkCore;

using System.Collections.Concurrent;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record UpdateChannelGroupCountsRequest(IEnumerable<ChannelGroupDto>? channelGroups = null) : IRequest { }


[LogExecutionTimeAspect]
public class UpdateChannelGroupCountsRequestHandler(ILogger<UpdateChannelGroupCountsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMemoryRequestHandler(logger, repository, mapper, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupCountsRequest>
{
    private class ChannelGroupBrief
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public async Task Handle(UpdateChannelGroupCountsRequest request, CancellationToken cancellationToken)
    {

        IEnumerable<ChannelGroupBrief> cgs;
        if (request.channelGroups != null)
        {
            IEnumerable<int> selectIds = request.channelGroups.Select(a => a.Id);
            cgs = Repository.ChannelGroup.GetAllChannelGroups().Where(a => selectIds.Contains(a.Id)).Select(a => new ChannelGroupBrief { Name = a.Name, Id = a.Id });
        }
        else
        {
            cgs = Repository.ChannelGroup.GetAllChannelGroups().Select(a => new ChannelGroupBrief { Name = a.Name, Id = a.Id });
        }

        //List<string> cgNames = cgs.Select(a => a.Name).Distinct().ToList();

        // Fetch all video streams once.
        var allVideoStreams = await Repository.VideoStream.GetAllVideoStreams()
            //.Where(a => cgNames.Contains(a.User_Tvg_group))
            .Select(vs => new
            {
                vs.Id,
                vs.User_Tvg_group,
                vs.IsHidden
            }).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        ConcurrentDictionary<string, List<string>> videoStreamsForGroups = new();
        ConcurrentDictionary<string, int> hiddenCounts = new();

        // Parallelizing the data processing.
        _ = Parallel.ForEach(cgs, cg =>
        {
            var relevantStreams = allVideoStreams.Where(vs => vs.User_Tvg_group == cg.Name).ToList();

            videoStreamsForGroups[cg.Name] = relevantStreams.Select(vs => vs.Id).ToList();
            hiddenCounts[cg.Name] = relevantStreams.Count(vs => vs.IsHidden);
        });

        List<ChannelGroupStreamCount> channelGroupStreamCounts = new();

        Task[] tasks = cgs.Select(async cg =>
        {
            ChannelGroupStreamCount response = new();

            HashSet<string> ids = new(videoStreamsForGroups[cg.Name]);
            int hiddenCount = hiddenCounts[cg.Name];

            response.TotalCount = ids.Count;
            response.ActiveCount = ids.Count - hiddenCount;
            response.HiddenCount = hiddenCount;
            response.Id = cg.Id;
            channelGroupStreamCounts.Add(response);

        }).ToArray();
        await Task.WhenAll(tasks);

        MemoryCache.AddOrUpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts);
    }
}
