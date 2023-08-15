using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

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
        Stopwatch stopwatch = new();
        stopwatch.Start();
        // Fetch only required channel groups.
        Func<ChannelGroup, bool> channelGroupPredicate = request.channelGroupNames == null ?
            (_ => true) :
            (a => request.channelGroupNames.Contains(a.Name));
        List<ChannelGroup> cgs = Repository.ChannelGroup.GetAllChannelGroups().Where(channelGroupPredicate).ToList();


        // Get the IQueryable for all video streams (this doesn't hit the database yet).
        IQueryable<VideoStream> allVideoStreamsQuery = Repository.VideoStream.GetAllVideoStreams();

        // Execute tasks in parallel
        Task[] tasks = cgs.Select(async cg =>
        {
            GetChannelGroupVideoStreamCountResponse response = new();

            List<VideoStream> videoStreamsForGroup = allVideoStreamsQuery.Where(vs => vs.User_Tvg_group == cg.Name).ToList();
            HashSet<string> ids = new(videoStreamsForGroup.Select(a => a.Id));
            int hiddenCount = videoStreamsForGroup.Count(a => a.IsHidden);

            if (!string.IsNullOrEmpty(cg.RegexMatch))
            {
                IEnumerable<VideoStream> reg = await Sender.Send(new GetVideoStreamsByNamePatternQuery(cg.RegexMatch), cancellationToken).ConfigureAwait(false);
                hiddenCount += reg.Count(a => a.IsHidden && !ids.Contains(a.Id));
                ids.UnionWith(reg.Select(a => a.Id));
            }

            response.TotalCount = ids.Count;
            response.ActiveCount = ids.Count - hiddenCount;
            response.HiddenCount = hiddenCount;
            response.Id = cg.Id;
            MemoryCache.AddOrUpdateChannelGroupVideoStreamCount(response);
        }).ToArray();

        await Task.WhenAll(tasks);

        stopwatch.Stop();
        Logger.LogInformation($"UpdateChannelGroupCountsRequest took {stopwatch.ElapsedMilliseconds} ms");

    }
}
