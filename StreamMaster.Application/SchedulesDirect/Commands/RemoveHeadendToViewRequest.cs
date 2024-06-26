using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveHeadendToViewRequest(string HeadendId, string Country, string Postal) : IRequest<APIResponse>;

public class RemoveHeadendToViewHandler(ISender Sender, IDataRefreshService dataRefreshService, IOptionsMonitor<SDSettings> intSDSettings)
: IRequestHandler<RemoveHeadendToViewRequest, APIResponse>
{
    private readonly SDSettings sdSettings = intSDSettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveHeadendToViewRequest request, CancellationToken cancellationToken)
    {
        if (!sdSettings.SDEnabled)
        {
            return APIResponse.ErrorWithMessage("SD is not enabled");
        }

        if (!sdSettings.HeadendsToView.Any(a => a.Id == request.HeadendId))
        {
            return APIResponse.Ok;
        }

        sdSettings.HeadendsToView.RemoveAll(a => a.Id == request.HeadendId);

        UpdateSettingParameters updateSetting = new()
        {
            SDSettings = new SDSettingsRequest
            {
                HeadendsToView = sdSettings.HeadendsToView
            }
        };
        _ = await Sender.Send(new UpdateSettingRequest(updateSetting), cancellationToken).ConfigureAwait(false);
        await dataRefreshService.Refresh("GetSubScribedHeadends");
        return APIResponse.Ok;
    }
}