using FluentValidation;

using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.Commands;

public class CreateVideoStreamRequestValidator : AbstractValidator<CreateVideoStreamRequest>
{
    public CreateVideoStreamRequestValidator()
    {
        _ = RuleFor(v => v.Tvg_name).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class CreateVideoStreamRequestHandler(ILogger<CreateVideoStreamRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<CreateVideoStreamRequest, VideoStreamDto?>
{
    public async Task<VideoStreamDto?> Handle(CreateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStream? stream = await Repository.VideoStream.CreateVideoStreamAsync(request, cancellationToken);

        if (stream != null)
        {
            VideoStreamDto streamDto = Mapper.Map<VideoStreamDto>(stream);
            await Publisher.Publish(new CreateVideoStreamEvent(streamDto), cancellationToken).ConfigureAwait(false);
            return streamDto;
        }

        return null;
    }
}