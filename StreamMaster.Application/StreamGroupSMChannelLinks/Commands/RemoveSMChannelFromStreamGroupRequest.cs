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

        SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        List<int> streamGroupIds = smChannel.StreamGroups.Select(a => a.StreamGroupId).ToList();

        FieldData fd = new(SMChannel.APIName, smChannel.Id, "StreamGroupIds", streamGroupIds);
        await dataRefreshService.SetField([fd]).ConfigureAwait(false);

        await dataRefreshService.ClearByTag(SMChannel.APIName, "notInSG").ConfigureAwait(false);
        await dataRefreshService.ClearByTag(SMChannel.APIName, "inSG").ConfigureAwait(false);
        await dataRefreshService.RefreshStreamGroups();
        await dataRefreshService.RefreshSMChannels();
        return res;
    }
}
