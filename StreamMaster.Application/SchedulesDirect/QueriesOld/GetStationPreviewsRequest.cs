namespace StreamMaster.Application.SchedulesDirect.QueriesOld;

public record GetStationPreviews : IRequest<List<StationPreview>>;

internal class GetStationPreviewsHandler(ILineups lineups, IOptionsMonitor<SDSettings> intsettings, ISender sender) : IRequestHandler<GetStationPreviews, List<StationPreview>>
{
    private readonly SDSettings settings = intsettings.CurrentValue;

    public async Task<List<StationPreview>> Handle(GetStationPreviews request, CancellationToken cancellationToken)
    {
        if (!settings.SDEnabled)
        {
            return [];
        }

        List<StationPreview> ret = await lineups.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
