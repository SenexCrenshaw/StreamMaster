namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupVideoStreamCount(string channelGropupName) : IRequest<ChannelGroupStreamCount?>;

internal class GetChannelGroupVideoStreamCountHandler(ILogger<GetChannelGroupVideoStreamCount> logger, ISender Sender, IMemoryCache MemoryCache) : IRequestHandler<GetChannelGroupVideoStreamCount, ChannelGroupStreamCount?>
{
    public async Task<ChannelGroupStreamCount?> Handle(GetChannelGroupVideoStreamCount request, CancellationToken cancellationToken)
    {
        ChannelGroupStreamCount res = new() { ActiveCount = 0, HiddenCount = 0 };
        ChannelGroupDto? cg = await Sender.Send(new GetChannelGroupByName(request.channelGropupName), cancellationToken).ConfigureAwait(false);
        return cg == null ? res : MemoryCache.GetChannelGroupVideoStreamCount(cg.Id);
    }
}