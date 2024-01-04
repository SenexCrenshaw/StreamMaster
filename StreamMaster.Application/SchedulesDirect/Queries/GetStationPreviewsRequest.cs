namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetStationPreviews : IRequest<List<StationPreview>>;

internal class GetStationPreviewsHandler(ILineups lineups, ISender sender) : IRequestHandler<GetStationPreviews, List<StationPreview>>
{
    public async Task<List<StationPreview>> Handle(GetStationPreviews request, CancellationToken cancellationToken)
    {
        SettingDto setting = await sender.Send(new GetSettings(), cancellationToken).ConfigureAwait(false);
        if (!setting.SDSettings.SDEnabled)
        {
            return [];
        }

        List<StationPreview> ret = await lineups.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
