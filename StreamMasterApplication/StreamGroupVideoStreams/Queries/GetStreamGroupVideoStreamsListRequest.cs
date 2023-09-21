namespace StreamMasterApplication.StreamGroupVideoStreams.Queries;

public record GetStreamGroupVideoStreamsListRequest(int StreamGroupId) : IRequest<List<VideoStreamDto>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupVideoStreamsListRequestHandler(ILogger<GetStreamGroupVideoStreamsListRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService) : BaseRequestHandler(logger, repository, mapper, settingsService), IRequestHandler<GetStreamGroupVideoStreamsListRequest, List<VideoStreamDto>>
{
    public async Task<List<VideoStreamDto>> Handle(GetStreamGroupVideoStreamsListRequest request, CancellationToken cancellationToken)
    {
        return await Repository.StreamGroupVideoStream.GetStreamGroupVideoStreams(request.StreamGroupId, cancellationToken).ConfigureAwait(false);
    }
}