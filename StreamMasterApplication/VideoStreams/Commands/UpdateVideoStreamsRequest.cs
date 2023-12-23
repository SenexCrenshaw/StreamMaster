using FluentValidation;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Requests;

namespace StreamMasterApplication.VideoStreams.Commands;

public class UpdateVideoStreamsRequestValidator : AbstractValidator<UpdateVideoStreamsRequest>
{
    public UpdateVideoStreamsRequestValidator()
    {
        _ = RuleFor(v => v.VideoStreamUpdates).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class UpdateVideoStreamsRequestHandler(ILogger<UpdateVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateVideoStreamsRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(UpdateVideoStreamsRequest requests, CancellationToken cancellationToken)
    {
        (List<VideoStreamDto> videoStreams, List<ChannelGroupDto>? updatedChannelGroups) = await Repository.VideoStream.UpdateVideoStreamsAsync(requests.VideoStreamUpdates, cancellationToken);
        if (videoStreams.Any())
        {
            bool toggleVisibilty = requests.VideoStreamUpdates.Any(x => x.ToggleVisibility.HasValue);
            if (toggleVisibilty)
            {
                await Publisher.Publish(new UpdateVideoStreamsEvent(new List<VideoStreamDto>()), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await Publisher.Publish(new UpdateVideoStreamsEvent(videoStreams), cancellationToken).ConfigureAwait(false);
            }

            if (updatedChannelGroups.Any())
            {
                updatedChannelGroups = await Sender.Send(new UpdateChannelGroupCountsRequest(updatedChannelGroups), cancellationToken).ConfigureAwait(false);
                await HubContext.Clients.All.ChannelGroupsRefresh(updatedChannelGroups.ToArray()).ConfigureAwait(false);
            }
        }

        return videoStreams;
    }
}