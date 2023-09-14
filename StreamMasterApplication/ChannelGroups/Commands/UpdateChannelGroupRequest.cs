using FluentValidation;

using StreamMasterApplication.ChannelGroups.Events;

namespace StreamMasterApplication.ChannelGroups.Commands;

public class UpdateChannelGroupRequestValidator : AbstractValidator<UpdateChannelGroupRequest>
{
    public UpdateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupId).NotNull().GreaterThan(0);
    }
}

public class UpdateChannelGroupRequestHandler(ILogger<UpdateChannelGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupRequest, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {

        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.ChannelGroupId).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }

        bool checkCounts = false;
        bool nameChanged = false;

        if (request.ToggleVisibility == true)
        {
            channelGroup.IsHidden = !channelGroup.IsHidden;
            List<VideoStreamDto> results = await Repository.VideoStream.SetGroupVisibleByGroupName(channelGroup.Name, channelGroup.IsHidden, cancellationToken).ConfigureAwait(false);

            checkCounts = results.Any();

        }

        if (!string.IsNullOrEmpty(request.NewGroupName) && request.NewGroupName != channelGroup.Name)
        {
            if (await Repository.ChannelGroup.GetChannelGroupByName(request.NewGroupName).ConfigureAwait(false) == null)
            {
                nameChanged = true;
                channelGroup.Name = request.NewGroupName;

            }
        }

        Repository.ChannelGroup.UpdateChannelGroup(channelGroup);
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        if (nameChanged && !string.IsNullOrEmpty(request.NewGroupName))
        {
            _ = await Repository.VideoStream.SetVideoStreamChannelGroupName(channelGroup.Name, request.NewGroupName, cancellationToken).ConfigureAwait(false);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
        }

        if (checkCounts || nameChanged)
        {

            ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
            if (checkCounts)
            {
                await Sender.Send(new UpdateChannelGroupCountRequest(dto, false), cancellationToken).ConfigureAwait(false);
            }

            await Publisher.Publish(new UpdateChannelGroupEvent(dto, request.ToggleVisibility ?? false, nameChanged), cancellationToken).ConfigureAwait(false);
        }

        return null;
    }
}
