using StreamMaster.Application.Settings.Commands;

namespace StreamMaster.Application.SchedulesDirect.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddHeadendToViewRequest(string HeadendId, string Country, string Postal) : IRequest<APIResponse>;

public class AddHeadendToViewHandler(IDataRefreshService dataRefreshService, ISender Sender, IOptionsMonitor<SDSettings> intSDSettings)
: IRequestHandler<AddHeadendToViewRequest, APIResponse>
{
    private readonly SDSettings sdSettings = intSDSettings.CurrentValue;

    public async Task<APIResponse> Handle(AddHeadendToViewRequest request, CancellationToken cancellationToken)
    {
        if (!sdSettings.SDEnabled)
        {
            return APIResponse.ErrorWithMessage("SD is not enabled");
        }

        if (sdSettings.HeadendsToView.Any(a => a.Id == request.HeadendId))
        {
            return APIResponse.Ok;
        }

        UpdateSettingParameters updateSetting = new()
        {
            SDSettings = new SDSettingsRequest
            {
                HeadendsToView = sdSettings.HeadendsToView
            }
        };

        HeadendToView headendToView = new()
        {
            Id = request.HeadendId,
            PostalCode = request.Postal,
            Country = request.Country
        };

        updateSetting.SDSettings.HeadendsToView.Add(headendToView);
        _ = await Sender.Send(new UpdateSettingRequest(updateSetting), cancellationToken).ConfigureAwait(false);
        await dataRefreshService.Refresh("GetSubScribedHeadends");

        return APIResponse.Ok;
    }
}