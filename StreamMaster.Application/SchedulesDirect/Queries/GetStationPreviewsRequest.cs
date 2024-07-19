namespace StreamMaster.Application.SchedulesDirect.Queries;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStationPreviewsRequest : IRequest<DataResponse<List<StationPreview>>>;

internal class GetStationPreviewsRequestHandler(ILineups lineups, IOptionsMonitor<SDSettings> intSettings)
    : IRequestHandler<GetStationPreviewsRequest, DataResponse<List<StationPreview>>>
{
    private readonly SDSettings settings = intSettings.CurrentValue;

    public async Task<DataResponse<List<StationPreview>>> Handle(GetStationPreviewsRequest request, CancellationToken cancellationToken)
    {
        if (!settings.SDEnabled)
        {
            return DataResponse<List<StationPreview>>.ErrorWithMessage("SD is not enabled");
        }

        List<StationPreview> ret = await lineups.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return DataResponse<List<StationPreview>>.Success(ret);
    }
}
