using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupVideoStreamCount(string channelGropupName) : IRequest<ChannelGroupStreamCount?>;

internal class GetChannelGroupVideoStreamCountHandler : BaseMemoryRequestHandler, IRequestHandler<GetChannelGroupVideoStreamCount, ChannelGroupStreamCount?>
{
    public GetChannelGroupVideoStreamCountHandler(ILogger<GetChannelGroupVideoStreamCountHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<ChannelGroupStreamCount?> Handle(GetChannelGroupVideoStreamCount request, CancellationToken cancellationToken)
    {
        ChannelGroupStreamCount res = new() { ActiveCount = 0, HiddenCount = 0 };
        ChannelGroupDto? cg = await Sender.Send(new GetChannelGroupByName(request.channelGropupName), cancellationToken).ConfigureAwait(false);
        if (cg == null)
        {
            return res;
        }

        return MemoryCache.GetChannelGroupVideoStreamCount(cg.Id);
    }
}