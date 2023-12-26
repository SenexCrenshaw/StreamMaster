using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.VideoStreams.Events;

namespace StreamMaster.Application.VideoStreams.Commands;

[RequireAll]
public record SetVideoStreamTimeShiftsRequest(List<string> Ids, string TimeShift) : IRequest { }

[LogExecutionTimeAspect]
public class SetVideoStreamTimeShiftsRequestHandler : BaseMediatorRequestHandler, IRequestHandler<SetVideoStreamTimeShiftsRequest>
{

    public SetVideoStreamTimeShiftsRequestHandler(ILogger<SetVideoStreamTimeShiftsRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
    : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task Handle(SetVideoStreamTimeShiftsRequest request, CancellationToken cancellationToken)
    {

        List<VideoStreamDto> streams = await Repository.VideoStream.SetVideoStreamTimeShiftsFromIds(request.Ids, request.TimeShift, cancellationToken).ConfigureAwait(false);
        if (streams.Any())
        {
            await Publisher.Publish(new UpdateVideoStreamsEvent(streams), cancellationToken).ConfigureAwait(false);
        }
    }
}