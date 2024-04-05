using StreamMaster.Application.ChannelGroups.Queries;

namespace StreamMaster.Application.ChannelGroups.QueriesOld;

public record GetChannelGroupVideoStreamCount(string channelGropupName) : IRequest<ChannelGroupStreamCount?>;

internal class GetChannelGroupVideoStreamCountHandler(ILogger<GetChannelGroupVideoStreamCount> logger, ISender Sender, IMemoryCache MemoryCache) : IRequestHandler<GetChannelGroupVideoStreamCount, ChannelGroupStreamCount?>
{
    public async Task<ChannelGroupStreamCount?> Handle(GetChannelGroupVideoStreamCount request, CancellationToken cancellationToken)
    {
        ChannelGroupStreamCount res = new() { ActiveCount = 0, HiddenCount = 0 };
        APIResponse<ChannelGroupDto?> cg = await Sender.Send(new GetChannelGroupByNameRequest(request.channelGropupName), cancellationToken).ConfigureAwait(false);
        return cg?.Data == null ? res : MemoryCache.GetChannelGroupVideoStreamCount(cg.Data.Id);
    }
}