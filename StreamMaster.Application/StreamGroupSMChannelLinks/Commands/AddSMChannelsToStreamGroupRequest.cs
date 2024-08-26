namespace StreamMaster.Application.StreamGroupSMChannelLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddSMChannelsToStreamGroupRequest(int StreamGroupId, List<int> SMChannelIds) : IRequest<APIResponse>;


internal class AddSMChannelsToStreamGroupRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<AddSMChannelsToStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddSMChannelsToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> fieldDatas = new();
        foreach (int smChannelId in request.SMChannelIds)
        {
            APIResponse res = await Repository.StreamGroupSMChannelLink.AddSMChannelToStreamGroup(request.StreamGroupId, smChannelId).ConfigureAwait(false);
            if (res.IsError)
            {
                return APIResponse.ErrorWithMessage(res.ErrorMessage);
            }

            SMChannel? smChannel = Repository.SMChannel.GetSMChannel(smChannelId);
            if (smChannel is null)
            {
                return APIResponse.ErrorWithMessage("Channel not found");
            }

            List<int> streamGroupIds = smChannel.StreamGroups.Select(a => a.StreamGroupId).ToList();

            fieldDatas.Add(new(SMChannel.APIName, smChannel.Id, "StreamGroupIds", streamGroupIds));
        }

        //APIResponse res = await Repositorywrapper.StreamGroupSMChannelLink.AddSMChannelsToStreamGroupAsync(request.StreamGroupId, request.SMChannelIds).ConfigureAwait(false);
        //if (res.IsErrored)
        //{
        //    return APIResponse.ErrorWithMessage(res.ErrorMessage);
        //}

        //SMChannel? SMChannel = Repositorywrapper.SMChannel.GetSMChannel(request.ShortSMChannelId);
        //if (SMChannel is null)
        //{
        //    return APIResponse.ErrorWithMessage("Channel not found");
        //}
        //List<int> streamGroupIds = SMChannel.StreamGroups.Select(a => a.StreamGroupId).ToList();

        //FieldData fd = new(SMChannel.APIName, SMChannel.Id, "StreamGroupIds", streamGroupIds);
        if (fieldDatas.Count > 0)
        {
            await dataRefreshService.SetField(fieldDatas).ConfigureAwait(false);

            await dataRefreshService.ClearByTag(SMChannel.APIName, "notInSG").ConfigureAwait(false);
            await dataRefreshService.ClearByTag(SMChannel.APIName, "inSG").ConfigureAwait(false);
        }
        await dataRefreshService.RefreshStreamGroups();
        return APIResponse.Success;
    }
}
