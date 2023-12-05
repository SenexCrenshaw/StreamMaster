using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStationPreviews : IRequest<List<StationPreview>>;

internal class GetStationPreviewsHandler(ISchedulesDirect schedulesDirect, ISender sender) : IRequestHandler<GetStationPreviews, List<StationPreview>>
{
    public async Task<List<StationPreview>> Handle(GetStationPreviews request, CancellationToken cancellationToken)
    {
        SettingDto setting = await sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);
        if (!setting.SDSettings.SDEnabled)
        {
            return new();
        }

        List<StationPreview> ret = await schedulesDirect.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
