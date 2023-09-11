namespace StreamMasterApplication.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamsListRequest(int StreamGroupId) : IRequest<List<VideoStream>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupVideoStreamsListRequestHandler(ILogger<GetStreamGroupVideoStreamsListRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetStreamGroupVideoStreamsListRequest, List<VideoStream>>
{
    public async Task<List<VideoStream>> Handle(GetStreamGroupVideoStreamsListRequest request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreamsList(request.StreamGroupId, cancellationToken).ConfigureAwait(false);
    }
}