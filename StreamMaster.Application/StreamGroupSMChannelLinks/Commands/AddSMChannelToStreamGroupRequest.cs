using StreamMaster.Application.Services;

namespace StreamMaster.Application.StreamGroupSMChannelLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddSMChannelToStreamGroupRequest(int StreamGroupId, int SMChannelId) : IRequest<APIResponse>;

internal class AddSMChannelToStreamGroupRequestHandler(IBackgroundTaskQueue taskQueue, IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<AddSMChannelToStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddSMChannelToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.StreamGroupSMChannelLink.AddSMChannelToStreamGroup(request.StreamGroupId, request.SMChannelId).ConfigureAwait(false);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (smChannel is null)
        {
            return APIResponse.ErrorWithMessage("Channel not found");
        }
        List<int> streamGroupIds = [.. smChannel.StreamGroups.Select(a => a.StreamGroupId)];

        FieldData fd = new(SMChannel.APIName, smChannel.Id, "StreamGroupIds", streamGroupIds);
        await dataRefreshService.SetField([fd]).ConfigureAwait(false);

        await dataRefreshService.ClearByTag(SMChannel.APIName, "notInSG").ConfigureAwait(false);
        await dataRefreshService.ClearByTag(SMChannel.APIName, "inSG").ConfigureAwait(false);
        await dataRefreshService.RefreshStreamGroups();
        await dataRefreshService.RefreshSMChannels();
        await taskQueue.CreateSTRMFiles(cancellationToken);
        return res;
    }
}