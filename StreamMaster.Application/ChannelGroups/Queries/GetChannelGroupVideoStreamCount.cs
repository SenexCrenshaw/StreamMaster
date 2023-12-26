using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupVideoStreamCount(string channelGropupName) : IRequest<ChannelGroupStreamCount?>;

internal class GetChannelGroupVideoStreamCountHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupVideoStreamCount, ChannelGroupStreamCount?>
{

    public GetChannelGroupVideoStreamCountHandler(ILogger<GetChannelGroupVideoStreamCount> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task<ChannelGroupStreamCount?> Handle(GetChannelGroupVideoStreamCount request, CancellationToken cancellationToken)
    {
        ChannelGroupStreamCount res = new() { ActiveCount = 0, HiddenCount = 0 };
        ChannelGroupDto? cg = await Sender.Send(new GetChannelGroupByName(request.channelGropupName), cancellationToken).ConfigureAwait(false);
        return cg == null ? res : MemoryCache.GetChannelGroupVideoStreamCount(cg.Id);
    }
}