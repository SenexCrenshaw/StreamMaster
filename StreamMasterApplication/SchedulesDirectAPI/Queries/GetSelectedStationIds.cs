using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetSelectedStationIds() : IRequest<List<StationIdLineup>>;

internal class GetSelectedStationIdsHandler(ISettingsService settingsService) : IRequestHandler<GetSelectedStationIds, List<StationIdLineup>>
{
    public async Task<List<StationIdLineup>> Handle(GetSelectedStationIds request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

        return setting.SDStationIds;
    }
}