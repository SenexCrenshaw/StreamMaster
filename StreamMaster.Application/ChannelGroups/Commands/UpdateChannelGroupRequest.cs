using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.StreamGroupChannelGroupLinks.Commands;

namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateChannelGroupRequest(int ChannelGroupId, string? NewGroupName, bool? IsHidden, bool? ToggleVisibility)
    : IRequest<APIResponse>
{ }

public class UpdateChannelGroupRequestHandler(IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher, ISender Sender)
    : IRequestHandler<UpdateChannelGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {

        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.ChannelGroupId).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return APIResponse.NotFound;
        }

        bool checkCounts = false;
        string? nameChanged = null;

        if (request.ToggleVisibility == true)
        {
            channelGroup.IsHidden = !channelGroup.IsHidden;
            List<VideoStreamDto> results = await Repository.VideoStream.SetGroupVisibleByGroupName(channelGroup.Name, channelGroup.IsHidden, cancellationToken).ConfigureAwait(false);

            checkCounts = results.Any();

        }

        if (!string.IsNullOrEmpty(request.NewGroupName) && request.NewGroupName != channelGroup.Name && await Repository.ChannelGroup.GetChannelGroupByName(request.NewGroupName).ConfigureAwait(false) == null)
        {
            nameChanged = channelGroup.Name;
            channelGroup.Name = request.NewGroupName;
        }

        Repository.ChannelGroup.UpdateChannelGroup(channelGroup);
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        if (nameChanged != null)
        {
            _ = await Repository.VideoStream.SetVideoStreamChannelGroupName(nameChanged, request.NewGroupName, cancellationToken).ConfigureAwait(false);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
        }

        await Sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(request.ChannelGroupId), cancellationToken).ConfigureAwait(false);

        if (checkCounts || nameChanged != null)
        {

            ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
            if (checkCounts)
            {
                await Sender.Send(new UpdateChannelGroupCountRequest(dto, false), cancellationToken).ConfigureAwait(false);
            }

            await Publisher.Publish(new UpdateChannelGroupEvent(dto, request.ToggleVisibility ?? false, nameChanged != null), cancellationToken).ConfigureAwait(false);
        }

        return APIResponse.Success;
    }
}
