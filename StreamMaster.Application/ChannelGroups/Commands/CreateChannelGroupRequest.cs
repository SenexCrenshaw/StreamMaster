using FluentValidation;

using StreamMaster.Application.ChannelGroups.Events;

namespace StreamMaster.Application.ChannelGroups.Commands;

[RequireAll]
public record CreateChannelGroupRequest(string GroupName, bool IsReadOnly) : IRequest<ChannelGroupDto?> { }

public class CreateChannelGroupRequestValidator : AbstractValidator<CreateChannelGroupRequest>
{
    public CreateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();

    }
}


public class CreateChannelGroupRequestHandler(ILogger<CreateChannelGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<CreateChannelGroupRequest, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(CreateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByName(request.GroupName).ConfigureAwait(false) != null)
        {
            return null;
        }

        ChannelGroup channelGroup = new() { Name = request.GroupName, IsReadOnly = request.IsReadOnly };
        Repository.ChannelGroup.CreateChannelGroup(channelGroup);
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        ChannelGroupDto channelGroupDto = Mapper.Map<ChannelGroupDto>(channelGroup);
        await Publisher.Publish(new CreateChannelGroupEvent(channelGroupDto), cancellationToken).ConfigureAwait(false);
        return channelGroupDto;
    }
}
