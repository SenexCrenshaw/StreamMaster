using AutoMapper;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.ChannelGroups.Queries;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Commands;

public record UpdateChannelGroupCountRequest(string channelGroupName) : IRequest
{

}

public class UpdateChannelGroupCountRequestValidator : AbstractValidator<UpdateChannelGroupCountRequest>
{
    public UpdateChannelGroupCountRequestValidator()
    {
        _ = RuleFor(v => v.channelGroupName).NotNull().NotEmpty();
    }
}

public class UpdateChannelGroupCountRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateChannelGroupCountRequest>
{

    public UpdateChannelGroupCountRequestHandler(ILogger<UpdateChannelGroupCountRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    { }

    public async Task Handle(UpdateChannelGroupCountRequest request, CancellationToken cancellationToken)
    {
        ChannelGroupDto? cg = await Sender.Send(new GetChannelGroupByName(request.channelGroupName), cancellationToken).ConfigureAwait(false);
        if (cg == null)
        {
            return;
        }

        ChannelGroupStreamCount response = new();
        IQueryable<VideoStream> allVideoStreamsQuery = Repository.VideoStream.GetAllVideoStreams();
        List<VideoStream> videoStreamsForGroup = allVideoStreamsQuery.Where(vs => vs.User_Tvg_group == cg.Name).ToList();
        // Filter video streams for the current channel group in memory.            
        HashSet<string> ids = new(videoStreamsForGroup.Select(a => a.Id));

        int hiddenCount = videoStreamsForGroup.Count(a => a.IsHidden);

        // Set the response values.
        response.TotalCount = ids.Count;
        response.ActiveCount = ids.Count - hiddenCount;
        response.HiddenCount = hiddenCount;
        response.Id = cg.Id;

        MemoryCache.AddOrUpdateChannelGroupVideoStreamCount(response);

    }
}
