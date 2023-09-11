﻿using FluentValidation;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record UpdateChannelGroupsRequest(List<UpdateChannelGroupRequest> ChannelGroupRequests) : IRequest
{
}

public class UpdateChannelGroupsRequestValidator : AbstractValidator<UpdateChannelGroupsRequest>
{
}

public class UpdateChannelGroupsRequestHandler : BaseMediatorRequestHandler, IRequestHandler<UpdateChannelGroupsRequest>
{

    public UpdateChannelGroupsRequestHandler(ILogger<UpdateChannelGroupsRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
 : base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }

    public async Task Handle(UpdateChannelGroupsRequest requests, CancellationToken cancellationToken)
    {
        //List<VideoStreamDto> results = new();

        foreach (UpdateChannelGroupRequest request in requests.ChannelGroupRequests)
        {

            await Sender.Send(new UpdateChannelGroupRequest(request.ChannelGroupId, request.NewGroupName, request.IsHidden, request.Rank, request.ToggleVisibility), cancellationToken).ConfigureAwait(false);

        }

        //await Publisher.Publish(new UpdateChannelGroupsEvent(), cancellationToken).ConfigureAwait(false);

        //if (results.Any())
        //{
        //    await Publisher.Publish(new UpdateVideoStreamsEvent(), cancellationToken).ConfigureAwait(false);
        //}

    }
}