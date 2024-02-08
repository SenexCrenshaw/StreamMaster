using FluentValidation;

using StreamMaster.Application.ChannelGroups.Events;
using StreamMaster.Application.StreamGroupChannelGroups.Commands;

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


public class CreateChannelGroupRequestHandler(ILogger<CreateChannelGroupRequest> logger, ISender sender, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher) : IRequestHandler<CreateChannelGroupRequest, ChannelGroupDto?>
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

        await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(channelGroup.Id), cancellationToken).ConfigureAwait(false);

        ChannelGroupDto channelGroupDto = Mapper.Map<ChannelGroupDto>(channelGroup);
        await Publisher.Publish(new CreateChannelGroupEvent(channelGroupDto), cancellationToken).ConfigureAwait(false);
        return channelGroupDto;
    }
}
