using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.ChannelGroups.Events;
using StreamMasterApplication.ChannelGroups.Queries;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

using StreamMasterDomain.Models;

namespace StreamMasterApplication.ChannelGroups.EventHandlers;

public class UpdateChannelGroupEventHandler(ILogger<UpdateChannelGroupEvent> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), INotificationHandler<UpdateChannelGroupEvent>
{
    public async Task Handle(UpdateChannelGroupEvent notification, CancellationToken cancellationToken)
    {
        if (notification.ChannelGroup == null)
        {
            return;
        }

        ChannelGroupDto? ret = await Sender.Send(new GetChannelGroup(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);

        if (ret == null)
        {
            return;
        }

        await HubContext.Clients.All.ChannelGroupsRefresh([ret]).ConfigureAwait(false);

        if (notification.ChannelGroupToggelVisibility)
        {
            await Sender.Send(new UpdateChannelGroupCountRequest(notification.ChannelGroup), cancellationToken).ConfigureAwait(false);
        }


        if (notification.ChannelGroupToggelVisibility || notification.ChannelGroupNameChanged)
        {
            List<VideoStream> streams = await repository.VideoStream.GetVideoStreamQuery().Where(a => a.User_Tvg_group == ret.Name).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            //streams = await Sender.Send(new GetVideoStreamsForChannelGroups([notification.ChannelGroup.Id]), cancellationToken).ConfigureAwait(false);

            if (streams.Any())
            {
                IEnumerable<IDIsHidden> ids = streams.Select(a => new IDIsHidden { Id = a.Id, IsHidden = a.IsHidden });
                await HubContext.Clients.All.VideoStreamsVisibilityRefresh(ids).ConfigureAwait(false);
            }
        }
        //else
        //{
        //    if (streams == null)
        //    {
        //        streams = await Sender.Send(new GetVideoStreamsForChannelGroups([notification.ChannelGroup.Id]), cancellationToken).ConfigureAwait(false);
        //    }

        //    if (streams != null)
        //    {
        //        if (notification.ChannelGroupNameChanged)
        //        {
        //            await HubContext.Clients.All.VideoStreamsRefresh(streams.ToArray()).ConfigureAwait(false);
        //        }
        //    }

        //}

        IEnumerable<StreamGroupDto> sgs = await Sender.Send(new GetStreamGroupsFromChannelGroupQuery(notification.ChannelGroup.Id), cancellationToken).ConfigureAwait(false);

        if (sgs.Any())
        {
            await HubContext.Clients.All.StreamGroupsRefresh(sgs.ToArray()).ConfigureAwait(false);
        }
    }
}