using FluentValidation;

namespace StreamMaster.Application.ChannelGroups.CommandsOld;

[RequireAll]
public record UpdateChannelGroupsRequest(List<UpdateChannelGroupRequest> ChannelGroupRequests) : IRequest
{
}

public class UpdateChannelGroupsRequestValidator : AbstractValidator<UpdateChannelGroupsRequest>
{
}

public class UpdateChannelGroupsRequestHandler(ILogger<UpdateChannelGroupsRequest> logger, ISender Sender) : IRequestHandler<UpdateChannelGroupsRequest>
{
    public async Task Handle(UpdateChannelGroupsRequest requests, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> results = [];

        foreach (UpdateChannelGroupRequest request in requests.ChannelGroupRequests)
        {
            results.Add(await Sender.Send(new UpdateChannelGroupRequest(request.ChannelGroupId, request.NewGroupName, request.IsHidden, request.ToggleVisibility), cancellationToken).ConfigureAwait(false));
        }

    }
}