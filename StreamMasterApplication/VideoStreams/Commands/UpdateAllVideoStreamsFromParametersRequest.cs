using FluentValidation;

using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record UpdateAllVideoStreamsFromParametersRequest(VideoStreamParameters Parameters, UpdateVideoStreamRequest Request, List<int>? ChannelGroupIds) : IRequest<List<VideoStreamDto>> { }

public class UpdateAllVideoStreamsFromParametersRequestValidator : AbstractValidator<UpdateAllVideoStreamsFromParametersRequest>
{
    public UpdateAllVideoStreamsFromParametersRequestValidator()
    {
        _ = RuleFor(v => v.Parameters).NotNull().NotEmpty();
    }
}

[LogExecutionTimeAspect]
public class UpdateAllVideoStreamsFromParametersRequestHandler(ILogger<UpdateAllVideoStreamsFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMemoryRequestHandler(logger, repository, mapper, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateAllVideoStreamsFromParametersRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(UpdateAllVideoStreamsFromParametersRequest request, CancellationToken cancellationToken)
    {

        (List<VideoStreamDto> videoStreams, bool updateChannelGroup) = await Repository.VideoStream.UpdateAllVideoStreamsFromParameters(request.Parameters, request.Request, cancellationToken);
        if (videoStreams.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(videoStreams, updateChannelGroup), cancellationToken).ConfigureAwait(false);
        }

        return videoStreams;
    }
}
