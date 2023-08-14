using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupVideoStreamCount(string channelGropupName) : IRequest<GetChannelGroupVideoStreamCountResponse?>;

internal class GetChannelGroupVideoStreamCountHandler : BaseMemoryRequestHandler, IRequestHandler<GetChannelGroupVideoStreamCount, GetChannelGroupVideoStreamCountResponse?>
{
    public GetChannelGroupVideoStreamCountHandler(ILogger<ProcessM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public async Task<GetChannelGroupVideoStreamCountResponse?> Handle(GetChannelGroupVideoStreamCount request, CancellationToken cancellationToken)
    {
        GetChannelGroupVideoStreamCountResponse res = new() { ActiveCount = 0, HiddenCount = 0 };
        ChannelGroupDto? cg = await Sender.Send(new GetChannelGroupByName(request.channelGropupName), cancellationToken).ConfigureAwait(false);
        if (cg == null)
        {
            return res;
        }

        return MemoryCache.GetChannelGroupVideoStreamCount(cg.Id);
    }
}