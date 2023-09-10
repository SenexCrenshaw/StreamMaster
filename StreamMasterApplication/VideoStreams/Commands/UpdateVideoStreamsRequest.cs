using FluentValidation;

using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;


public class UpdateVideoStreamsRequestValidator : AbstractValidator<UpdateVideoStreamsRequest>
{
    public UpdateVideoStreamsRequestValidator()
    {
        _ = RuleFor(v => v.VideoStreamUpdates).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class UpdateVideoStreamsRequestHandler(ILogger<UpdateVideoStreamsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMemoryRequestHandler(logger, repository, mapper, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateVideoStreamsRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(UpdateVideoStreamsRequest requests, CancellationToken cancellationToken)
    {

        (List<VideoStreamDto> videoStreams, bool updateChannelGroup) = await Repository.VideoStream.UpdateVideoStreamsAsync(requests.VideoStreamUpdates, cancellationToken);
        if (videoStreams.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(videoStreams, updateChannelGroup), cancellationToken).ConfigureAwait(false);
        }

        return videoStreams;
    }
}
