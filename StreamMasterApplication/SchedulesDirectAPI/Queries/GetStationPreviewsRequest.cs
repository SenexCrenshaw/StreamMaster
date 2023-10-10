using StreamMaster.SchedulesDirectAPI;

using StreamMasterApplication.Services;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStationPreviewsRequest : IRequest<List<StationPreview>>;

internal class GetStationPreviewsRequestHandler(ISDService schedulesDirect) : IRequestHandler<GetStationPreviewsRequest, List<StationPreview>>
{
    public async Task<List<StationPreview>> Handle(GetStationPreviewsRequest request, CancellationToken cancellationToken)
    {
        List<StationPreview> ret = await schedulesDirect.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
