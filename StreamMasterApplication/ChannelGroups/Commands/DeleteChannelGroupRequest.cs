using FluentValidation;

using StreamMasterApplication.ChannelGroups.Events;

namespace StreamMasterApplication.ChannelGroups.Commands;

[RequireAll]
public record DeleteChannelGroupRequest(int channelGroupId) : IRequest<bool>
{
}

public class DeleteChannelGroupRequestValidator : AbstractValidator<DeleteChannelGroupRequest>
{
    public DeleteChannelGroupRequestValidator()
    {
        _ = RuleFor(v => v.channelGroupId).NotNull().GreaterThan(0);
    }
}

public class DeleteChannelGroupRequestHandler(ILogger<DeleteChannelGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<DeleteChannelGroupRequest, bool>
{
    public async Task<bool> Handle(DeleteChannelGroupRequest request, CancellationToken cancellationToken)
    {
        //ChannelGroupDto? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.channelGroupId).ConfigureAwait(false);

        //if (channelGroup == null)
        //{
        //    return false;
        //}

        (int? ChannelGroupId, IEnumerable<VideoStreamDto> VideoStreams) = await Repository.ChannelGroup.DeleteChannelGroupById(request.channelGroupId).ConfigureAwait(false); ;

        if (ChannelGroupId == null)
        {
            return false;
        }


        await Publisher.Publish(new DeleteChannelGroupEvent((int)ChannelGroupId, VideoStreams), cancellationToken);

        return true;
    }
}
