using StreamMasterApplication.VideoStreams.Events;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.VideoStreams.Commands;

public record SetVideoStreamTimeShiftFromParametersRequest(VideoStreamParameters Parameters, string TimeShift) : IRequest<List<VideoStreamDto>> { }

[LogExecutionTimeAspect]
public class SetVideoStreamTimeShiftFromParametersRequestHandler : BaseMediatorRequestHandler, IRequestHandler<SetVideoStreamTimeShiftFromParametersRequest, List<VideoStreamDto>>
{

    public SetVideoStreamTimeShiftFromParametersRequestHandler(ILogger<SetVideoStreamTimeShiftFromParametersRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task<List<VideoStreamDto>> Handle(SetVideoStreamTimeShiftFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<VideoStreamDto> results = await Repository.VideoStream.SetVideoStreamTimeShiftFromParameters(request.Parameters, request.TimeShift, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(results), cancellationToken).ConfigureAwait(false);
        }
        return results;
    }
}