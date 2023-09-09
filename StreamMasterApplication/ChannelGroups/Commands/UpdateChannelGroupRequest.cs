using FluentValidation;

using StreamMasterApplication.ChannelGroups.Events;

namespace StreamMasterApplication.ChannelGroups.Commands;

public class UpdateChannelGroupRequestValidator : AbstractValidator<UpdateChannelGroupRequest>
{
    public UpdateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupId).NotNull().GreaterThan(0);
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class UpdateChannelGroupRequestHandler(ILogger<UpdateChannelGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<UpdateChannelGroupRequest>
{
    public async Task Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {

        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.ChannelGroupId).ConfigureAwait(false);
        bool checkCounts = false;
        if (channelGroup == null)
        {
            return;
        }

        if (request.Rank != null && (int)request.Rank != request.Rank)
        {
            channelGroup.Rank = (int)request.Rank;
        }

        if (request.ToggleVisibility == true)
        {
            List<VideoStreamDto> results = await Repository.VideoStream.SetGroupVisibleByGroupName(channelGroup.Name, !channelGroup.IsHidden, cancellationToken).ConfigureAwait(false);
            channelGroup.IsHidden = !channelGroup.IsHidden;
            checkCounts = results.Any();

        }

        if (!string.IsNullOrEmpty(request.NewGroupName) && request.NewGroupName != channelGroup.Name)
        {
            if (await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.NewGroupName).ConfigureAwait(false) == null)
            {
                channelGroup.Name = request.NewGroupName;
                _ = await Repository.VideoStream.SetGroupNameByGroupName(channelGroup.Name, request.NewGroupName, cancellationToken).ConfigureAwait(false);
            }
        }

        Repository.ChannelGroup.UpdateChannelGroup(channelGroup);
        _ = await Repository.SaveAsync().ConfigureAwait(false);


        if (checkCounts)
        {
            ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
            await Publisher.Publish(new UpdateChannelGroupEvent(dto), cancellationToken).ConfigureAwait(false);
        }


    }
}
