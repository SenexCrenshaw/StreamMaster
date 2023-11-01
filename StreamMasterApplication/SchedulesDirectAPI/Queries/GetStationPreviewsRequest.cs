using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStationPreviewsRequest : IRequest<List<StationPreview>>;

internal class GetStationPreviewsRequestHandler(ISDService schedulesDirect, ISender sender) : IRequestHandler<GetStationPreviewsRequest, List<StationPreview>>
{
    public async Task<List<StationPreview>> Handle(GetStationPreviewsRequest request, CancellationToken cancellationToken)
    {
        SettingDto setting = await sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return new();
        }

        List<StationPreview> ret = await schedulesDirect.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
