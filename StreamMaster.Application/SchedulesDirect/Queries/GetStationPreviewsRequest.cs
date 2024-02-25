using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetStationPreviews : IRequest<List<StationPreview>>;

internal class GetStationPreviewsHandler(ILineups lineups, IOptionsMonitor<Setting> intsettings, ISender sender) : IRequestHandler<GetStationPreviews, List<StationPreview>>
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<List<StationPreview>> Handle(GetStationPreviews request, CancellationToken cancellationToken)
    {
        if (!settings.SDSettings.SDEnabled)
        {
            return [];
        }

        List<StationPreview> ret = await lineups.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
