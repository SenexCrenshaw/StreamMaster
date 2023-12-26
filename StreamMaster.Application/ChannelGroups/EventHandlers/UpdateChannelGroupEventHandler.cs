using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.StreamGroupChannelGroups.Queries;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler(ILogger<UpdateChannelGroupEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<UpdateChannelGroupEvent>
{
    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {

        if (notification.ChannelGroupToggelVisibility || notification.ChannelGroupNameChanged)
        {
            await Sender.Send(new UpdateChannelGroupCountRequest(notification.ChannelGroup, true), cancellationToken).ConfigureAwait(false);
            await HubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);

            List<VideoStreamDto> streams = await repository.VideoStream.GetVideoStreamsForChannelGroup(notification.ChannelGroup.Id, cancellationToken).ConfigureAwait(false);
            if (streams.Any())
            {
                await HubContext.Clients.All.VideoStreamsRefresh([.. streams]).ConfigureAwait(false);
            }


            IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupQuery(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);

            if (sgs.Any())
            {
                await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
            }
        }
        else
        {
            await HubContext.Clients.All.ChannelGroupsRefresh([notification.ChannelGroup]).ConfigureAwait(false);
        }
    }
}