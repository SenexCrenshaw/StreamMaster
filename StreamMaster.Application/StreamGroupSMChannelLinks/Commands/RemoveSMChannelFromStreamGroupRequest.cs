namespace StreamMaster.Application.StreamGroupSMChannelLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveSMChannelFromStreamGroupRequest(int StreamGroupId, int SMChannelId) : IRequest<APIResponse>;

internal class RemoveSMChannelFromStreamGroupRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<RemoveSMChannelFromStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveSMChannelFromStreamGroupRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.StreamGroupSMChannelLink.RemoveSMChannelFromStreamGroup(request.StreamGroupId, request.SMChannelId).ConfigureAwait(false);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.ClearByTag(SMChannel.MainGet, "notInSG").ConfigureAwait(false);
        await dataRefreshService.ClearByTag(SMChannel.MainGet, "inSG").ConfigureAwait(false);
        await dataRefreshService.RefreshStreamGroupSMChannelLinks().ConfigureAwait(false);
        //await hubContext.Clients.All.ClearByTag(new ClearByTag("GetPagedSMChannes", "IsHidden")).ConfigureAwait(false);
        //await hubContext.Clients.All.DataRefresh("StreamGroupSMChannelLinks").ConfigureAwait(false);
        return res;
    }
}
