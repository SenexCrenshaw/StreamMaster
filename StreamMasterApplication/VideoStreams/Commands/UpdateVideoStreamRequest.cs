using FluentValidation;

using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.VideoStreams.Events;
using StreamMasterDomain.Models;

namespace StreamMasterApplication.VideoStreams.Commands;

public class UpdateVideoStreamRequestValidator : AbstractValidator<UpdateVideoStreamRequest>
{
    public UpdateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class UpdateVideoStreamRequestHandler(ILogger<UpdateVideoStreamRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateVideoStreamRequest, VideoStreamDto?>
{
    public async Task<VideoStreamDto?> Handle(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {

        (VideoStreamDto? videoStream, ChannelGroupDto? updateChannelGroup) = await Repository.VideoStream.UpdateVideoStreamAsync(request, cancellationToken).ConfigureAwait(false);
        if (videoStream is not null)
        {
            await Publisher.Publish(new UpdateVideoStreamEvent(videoStream, request.ToggleVisibility ?? false), cancellationToken).ConfigureAwait(false);

            if (updateChannelGroup != null)
            {

                await Publisher.Publish(new UpdateChannelGroupCountRequest(updateChannelGroup)).ConfigureAwait(false);

            }
        }
        return videoStream;
    }
}
