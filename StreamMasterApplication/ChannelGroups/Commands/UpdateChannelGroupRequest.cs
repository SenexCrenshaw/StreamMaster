using FluentValidation;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.ChannelGroups.Events;

using StreamMasterDomain.Models;

namespace StreamMasterApplication.ChannelGroups.Commands;

public class UpdateChannelGroupRequestValidator : AbstractValidator<UpdateChannelGroupRequest>
{
    public UpdateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.ChannelGroupId).NotNull().GreaterThan(0);
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}

public class UpdateChannelGroupRequestHandler(ILogger<UpdateChannelGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateChannelGroupRequest, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(UpdateChannelGroupRequest request, CancellationToken cancellationToken)
    {

        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupQuery().FirstOrDefaultAsync(a => a.Id == request.ChannelGroupId, cancellationToken: cancellationToken).ConfigureAwait(false);
        bool checkCounts = false;
        bool nameChanged = false;

        if (channelGroup == null)
        {
            return null;
        }

        if (request.ToggleVisibility == true)
        {
            List<VideoStreamDto> results = await Repository.VideoStream.SetGroupVisibleByGroupName(channelGroup.Name, channelGroup.IsHidden, cancellationToken).ConfigureAwait(false);
            channelGroup.IsHidden = !channelGroup.IsHidden;
            checkCounts = results.Any();

        }

        if (!string.IsNullOrEmpty(request.NewGroupName) && request.NewGroupName != channelGroup.Name)
        {
            if (await Repository.ChannelGroup.GetChannelGroupByName(request.NewGroupName).ConfigureAwait(false) == null)
            {
                nameChanged = true;
                channelGroup.Name = request.NewGroupName;
                _ = await Repository.VideoStream.SetChannelGroupNameByGroupName(channelGroup.Name, request.NewGroupName, cancellationToken).ConfigureAwait(false);
            }
        }

        Repository.ChannelGroup.UpdateChannelGroup(channelGroup);
        _ = await Repository.SaveAsync().ConfigureAwait(false);


        if (checkCounts || nameChanged)
        {
            ChannelGroupDto dto = Mapper.Map<ChannelGroupDto>(channelGroup);
            await Publisher.Publish(new UpdateChannelGroupEvent(dto, request.ToggleVisibility ?? false, nameChanged), cancellationToken).ConfigureAwait(false);
        }

        return null;
    }
}
