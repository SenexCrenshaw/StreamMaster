using StreamMaster.Application.StreamGroupChannelGroups.Commands;

namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[RequireAll]
public record CreateChannelGroupRequest(string GroupName, bool IsReadOnly) : IRequest<DefaultAPIResponse> { }

public class CreateChannelGroupRequestHandler(ILogger<CreateChannelGroupRequest> logger, IMessageService messageSevice, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ISender sender, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher) : IRequestHandler<CreateChannelGroupRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(CreateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByName(request.GroupName).ConfigureAwait(false) != null)
        {
            return APIResponseFactory.NotFound;
        }

        ChannelGroupDto? channelGroupDto = await Repository.ChannelGroup.CreateChannelGroup(request.GroupName, request.IsReadOnly);
        if (channelGroupDto == null)
        {
            return APIResponseFactory.NotFound;
        }

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(channelGroupDto.Id), cancellationToken).ConfigureAwait(false);

        await hubContext.Clients.All.DataRefresh("ChannelGroupDto").ConfigureAwait(false);
        await messageSevice.SendSuccess("Created CG '" + channelGroupDto.Name);
        return APIResponseFactory.Ok;
    }
}
