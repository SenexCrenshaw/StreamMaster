namespace StreamMaster.Application.SchedulesDirect.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSelectedStationIdsRequest : IRequest<DataResponse<List<StationIdLineup>>>;

internal class GetSelectedStationIdsRequestHandler(IOptionsMonitor<SDSettings> intSettings)
    : IRequestHandler<GetSelectedStationIdsRequest, DataResponse<List<StationIdLineup>>>
{
    private readonly SDSettings settings = intSettings.CurrentValue;
    public Task<DataResponse<List<StationIdLineup>>> Handle(GetSelectedStationIdsRequest request, CancellationToken cancellationToken)
    {

        return Task.FromResult(DataResponse<List<StationIdLineup>>.Success(settings.SDStationIds.OrderBy(a => a.StationId, StringComparer.OrdinalIgnoreCase).ToList()));
    }
}
