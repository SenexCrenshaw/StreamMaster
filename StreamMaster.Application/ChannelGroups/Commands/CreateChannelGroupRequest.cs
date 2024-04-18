using StreamMaster.Application.StreamGroupChannelGroupLinks.Commands;

namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateChannelGroupRequest(string GroupName, bool IsReadOnly) : IRequest<APIResponse> { }

public class CreateChannelGroupRequestHandler(IMessageService messageSevice, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ISender sender, IRepositoryWrapper Repository)
    : IRequestHandler<CreateChannelGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByName(request.GroupName).ConfigureAwait(false) != null)
        {
            return APIResponse.NotFound;
        }

        ChannelGroupDto? channelGroupDto = await Repository.ChannelGroup.CreateChannelGroup(request.GroupName, request.IsReadOnly);
        if (channelGroupDto == null)
        {
            return APIResponse.NotFound;
        }

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(channelGroupDto.Id), cancellationToken).ConfigureAwait(false);

        await hubContext.Clients.All.DataRefresh("GetPagedChannelGroups").ConfigureAwait(false);
        await messageSevice.SendSuccess("Created CG '" + channelGroupDto.Name);
        return APIResponse.Success;
    }
}
