using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

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

    public UpdateChannelGroupCountsRequestHandler(ILogger<ProcessM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    { }

    public async Task Handle(UpdateChannelGroupCountsRequest request, CancellationToken cancellationToken)
    {

        // Fetch all channel groups.
        List<ChannelGroup> cgsData = Repository.ChannelGroup.GetAllChannelGroups().ToList();

        List<ChannelGroup> cgs = cgsData.Where(a => request.channelGroupNames == null || request.channelGroupNames.Contains(a.Name)).ToList();

        // Get the IQueryable for all video streams (this doesn't hit the database yet).
        IQueryable<VideoStream> allVideoStreamsQuery = Repository.VideoStream.GetAllVideoStreams();


        foreach (ChannelGroup cg in cgs)
        {
            GetChannelGroupVideoStreamCountResponse response = new();

            List<VideoStream> videoStreamsForGroup = allVideoStreamsQuery.Where(vs => vs.User_Tvg_group == cg.Name).ToList();
            // Filter video streams for the current channel group in memory.            
            HashSet<string> ids = new(videoStreamsForGroup.Select(a => a.Id));

            int hiddenCount = videoStreamsForGroup.Count(a => a.IsHidden);

            // If a regex match pattern is defined for the channel group, fetch additional video streams.
            if (!string.IsNullOrEmpty(cg.RegexMatch))
            {
                IEnumerable<VideoStream> reg = await Sender.Send(new GetVideoStreamsByNamePatternQuery(cg.RegexMatch), cancellationToken).ConfigureAwait(false);
                hiddenCount += reg.Count(a => a.IsHidden && !ids.Contains(a.Id));
                ids.UnionWith(reg.Select(a => a.Id));
            }

            // Set the response values.
            response.TotalCount = ids.Count;
            response.ActiveCount = ids.Count - hiddenCount;
            response.HiddenCount = hiddenCount;
            response.Id = cg.Id;
            MemoryCache.AddOrUpdateChannelGroupVideoStreamCount(response);
        }

    }
}
