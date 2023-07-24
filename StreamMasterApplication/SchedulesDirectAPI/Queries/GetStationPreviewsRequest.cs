using MediatR;

using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStationPreviewsRequest : IRequest<List<StationPreview>>;

internal class GetStationPreviewsRequestHandler : IRequestHandler<GetStationPreviewsRequest, List<StationPreview>>
{
    public async Task<List<StationPreview>> Handle(GetStationPreviewsRequest request, CancellationToken cancellationToken)
    {
        var sd = new SchedulesDirect();

        var ret = await sd.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
