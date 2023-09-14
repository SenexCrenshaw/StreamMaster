using FluentValidation;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record UpdateChannelGroupsRequest(List<UpdateChannelGroupRequest> ChannelGroupRequests) : IRequest
{
}

public class UpdateChannelGroupsRequestValidator : AbstractValidator<UpdateChannelGroupsRequest>
{
}

public class UpdateChannelGroupsRequestHandler(ILogger<UpdateChannelGroupsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupsRequest>
{
    public async Task Handle(UpdateChannelGroupsRequest requests, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> results = new();

        foreach (UpdateChannelGroupRequest request in requests.ChannelGroupRequests)
        {
            results.Add(await Sender.Send(new UpdateChannelGroupRequest(request.ChannelGroupId, request.NewGroupName, request.IsHidden, request.ToggleVisibility), cancellationToken).ConfigureAwait(false));
        }

        //if (results.Any())
        //{
        //    await Publisher.Publish(new UpdateChannelGroupsEvent(results), cancellationToken).ConfigureAwait(false);
        //}

        //if (results.Any())
        //{
        //    await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        //}

    }
}