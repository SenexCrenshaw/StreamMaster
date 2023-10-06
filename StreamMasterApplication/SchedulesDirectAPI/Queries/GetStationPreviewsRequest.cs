using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStationPreviewsRequest : IRequest<List<StationPreview>>;

internal class GetStationPreviewsRequestHandler(ISettingsService settingsService) : IRequestHandler<GetStationPreviewsRequest, List<StationPreview>>
{
    public async Task<List<StationPreview>> Handle(GetStationPreviewsRequest request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        List<StationPreview> ret = await sd.GetStationPreviews(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
