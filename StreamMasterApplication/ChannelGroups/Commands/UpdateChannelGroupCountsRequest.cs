using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record UpdateChannelGroupCountsRequest(IEnumerable<string>? channelGroupNames = null) : IRequest
{
}

public class UpdateChannelGroupCountsRequestValidator : AbstractValidator<UpdateChannelGroupCountsRequest>
{
    public UpdateChannelGroupCountsRequestValidator()
    {
    }
}

public class UpdateChannelGroupCountsRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateChannelGroupCountsRequest>
{

    public UpdateChannelGroupCountsRequestHandler(ILogger<UpdateChannelGroupCountsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    { }

    public async Task Handle(UpdateChannelGroupCountsRequest request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("UpdateChannelGroupCountsRequestHandler.Handle - Start");
        Stopwatch stopwatch = new();
        stopwatch.Start();

        //// Fetch only required channel groups.
        //Func<ChannelGroup, bool> channelGroupPredicate = request.channelGroupNames == null ?
        //(_ => true) :
        //(a => request.channelGroupNames.Contains(a.Name));
        List<ChannelGroup> cgs = Repository.ChannelGroup.GetAllChannelGroups().Where(a => request.channelGroupNames == null || request.channelGroupNames.Contains(a.Name)).ToList();

        // Fetch all video streams once.
        var allVideoStreams = await Repository.VideoStream.GetAllVideoStreams()
            .Select(vs => new
            {
                vs.Id,
                vs.User_Tvg_group,
                vs.IsHidden
            }).ToListAsync().ConfigureAwait(false);

        ConcurrentDictionary<string, List<string>> videoStreamsForGroups = new();
        ConcurrentDictionary<string, int> hiddenCounts = new();

        // Parallelizing the data processing.
        Parallel.ForEach(cgs, cg =>
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

            if (!string.IsNullOrEmpty(cg.RegexMatch))
            {
                IEnumerable<VideoStreamDto> reg = await Sender.Send(new GetVideoStreamsByNamePatternQuery(cg.RegexMatch), cancellationToken).ConfigureAwait(false);
                hiddenCount += reg.Count(a => a.IsHidden && !ids.Contains(a.Id));
                ids.UnionWith(reg.Select(a => a.Id));
            }

            response.TotalCount = ids.Count;
            response.ActiveCount = ids.Count - hiddenCount;
            response.HiddenCount = hiddenCount;
            response.Id = cg.Id;
            channelGroupStreamCounts.Add(response);

        }).ToArray();
        await Task.WhenAll(tasks);

        await Repository.ChannelGroup.AddOrUpdateChannelGroupVideoStreamCounts(channelGroupStreamCounts).ConfigureAwait(false);

        await Repository.SaveAsync().ConfigureAwait(false);

        stopwatch.Stop();
        Logger.LogInformation($"UpdateChannelGroupCountsRequest took {stopwatch.ElapsedMilliseconds} ms");
    }


}
