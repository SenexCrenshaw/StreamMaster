namespace StreamMaster.Application.StreamGroupSMChannelLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddSMChannelsToStreamGroupByParametersRequest(QueryStringParameters Parameters, int StreamGroupId) : IRequest<APIResponse>;


internal class AddSMChannelsToStreamGroupByParametersRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<AddSMChannelsToStreamGroupByParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddSMChannelsToStreamGroupByParametersRequest request, CancellationToken cancellationToken)
    {
        var fieldDatas = new List<FieldData>();
        var smChannels = await Repository.SMChannel.GetPagedSMChannelsQueryable(request.Parameters);
        foreach (var smChannel in smChannels)
        {
            APIResponse res = await Repository.StreamGroupSMChannelLink.AddSMChannelToStreamGroup(request.StreamGroupId, smChannel.Id).ConfigureAwait(false);
            if (res.IsError)
            {
                return APIResponse.ErrorWithMessage(res.ErrorMessage);
            }

            List<int> streamGroupIds = smChannel.StreamGroups.Select(a => a.StreamGroupId).ToList();

            fieldDatas.Add(new(SMChannel.APIName, smChannel.Id, "StreamGroupIds", streamGroupIds));
        }

        if (fieldDatas.Count > 0)
        {
            await dataRefreshService.SetField(fieldDatas).ConfigureAwait(false);

            await dataRefreshService.ClearByTag(SMChannel.APIName, "notInSG").ConfigureAwait(false);
            await dataRefreshService.ClearByTag(SMChannel.APIName, "inSG").ConfigureAwait(false);
        }
        return APIResponse.Success;
    }
}
