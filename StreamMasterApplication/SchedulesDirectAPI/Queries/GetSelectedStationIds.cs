namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetSelectedStationIds() : IRequest<List<StationIdLineUp>>;

internal class GetSelectedStationIdsHandler(ISettingsService settingsService) : IRequestHandler<GetSelectedStationIds, List<StationIdLineUp>>
{
    public async Task<List<StationIdLineUp>> Handle(GetSelectedStationIds request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();

        return setting.SDStationIds;
    }
}
