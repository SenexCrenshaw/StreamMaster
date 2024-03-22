using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.StreamGroupChannelGroups.Commands;

namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[RequireAll]
public record CreateChannelGroupRequest(string GroupName, bool IsReadOnly) : IRequest<ChannelGroupDto?> { }

public class CreateChannelGroupRequestHandler(ILogger<CreateChannelGroupRequest> logger, ISender sender, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher) : IRequestHandler<CreateChannelGroupRequest, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(CreateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByName(request.GroupName).ConfigureAwait(false) != null)
        {
            return null;
        }

        ChannelGroupDto? channelGroupDto = await Repository.ChannelGroup.CreateChannelGroup(request.GroupName, request.IsReadOnly);
        if (channelGroupDto == null)
        {
            return null;
        }

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(channelGroupDto.Id), cancellationToken).ConfigureAwait(false);

        await Publisher.Publish(new CreateChannelGroupEvent(channelGroupDto), cancellationToken).ConfigureAwait(false);
        return channelGroupDto;
    }
}
