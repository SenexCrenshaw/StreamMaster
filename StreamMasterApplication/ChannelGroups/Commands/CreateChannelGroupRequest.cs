using FluentValidation;

using StreamMasterApplication.ChannelGroups.Events;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record CreateChannelGroupRequest(string GroupName, int Rank, bool IsReadOnly) : IRequest<ChannelGroupDto?> { }

public class CreateChannelGroupRequestValidator : AbstractValidator<CreateChannelGroupRequest>
{
    public CreateChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.GroupName).NotNull().NotEmpty();
        _ = RuleFor(v => v.Rank).NotNull().GreaterThan(0);
    }
}


[LogExecutionTimeAspect]
public class CreateChannelGroupRequestHandler(ILogger<CreateChannelGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<CreateChannelGroupRequest, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(CreateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.GroupName).ConfigureAwait(false) != null)
        {
            return null;
        }

        ChannelGroup channelGroup = new() { Name = request.GroupName, Rank = request.Rank, IsReadOnly = request.IsReadOnly };
        Repository.ChannelGroup.CreateChannelGroup(channelGroup);
        _ = await Repository.SaveAsync().ConfigureAwait(false);

        ChannelGroupDto channelGroupDto = Mapper.Map<ChannelGroupDto>(channelGroup);
        await Publisher.Publish(new CreateChannelGroupEvent(channelGroupDto), cancellationToken).ConfigureAwait(false);
        return channelGroupDto;
    }
}
