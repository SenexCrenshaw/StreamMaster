using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.StreamGroupChannelGroupLinks.Queries;

namespace StreamMaster.Application.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler(ILogger<UpdateChannelGroupEvent> logger, IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<UpdateChannelGroupEvent>
{
    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        //await HubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);

        if (notification.ChannelGroupToggelVisibility || notification.ChannelGroupNameChanged)
        {
            //await Sender.Send(new UpdateChannelGroupCountRequest(notification.ChannelGroup, true), cancellationToken).ConfigureAwait(false);
            //await HubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);

            List<VideoStreamDto> streams = await Repository.VideoStream.GetVideoStreamsForChannelGroup(notification.ChannelGroup.Id, cancellationToken).ConfigureAwait(false);
            if (streams.Any())
            {
                await HubContext.Clients.All.VideoStreamsRefresh([.. streams]).ConfigureAwait(false);
            }


            //IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupQuery(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);

            //if (sgs.Any())
            //{
            //    await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
            //}
        }
        //else
        //{

        //}
        await Sender.Send(new UpdateChannelGroupCountRequest(notification.ChannelGroup, true), cancellationToken).ConfigureAwait(false);
        //await HubContext.Clients.All.ChannelGroupsRefresh([notification.ChannelGroup]).ConfigureAwait(false);
        IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupQuery(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);

        if (sgs.Any())
        {
            await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
        }
    }
}