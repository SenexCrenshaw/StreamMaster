namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AutoSetSMChannelNumbersFromParametersRequest(int StreamGroupId, QueryStringParameters Parameters, int? StartingNumber, bool? OverwriteExisting) : IRequest<APIResponse>;

internal class AutoSetSMChannelNumbersFromParametersRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<AutoSetSMChannelNumbersFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AutoSetSMChannelNumbersFromParametersRequest request, CancellationToken cancellationToken)
    {

        IdIntResultWithResponse res = await Repository.SMChannel.AutoSetSMChannelNumbersFromParameters(request.StreamGroupId, request.Parameters, request.StartingNumber, request.OverwriteExisting);
        if (res.APIResponse.IsError)
        {
            return APIResponse.ErrorWithMessage(res.APIResponse.ErrorMessage);
        }
        List<FieldData> ret = [];
        foreach (IdIntResult item in res)
        {
            if (item.Result is SMChannel a)
            {
                ret.Add(new FieldData(SMChannel.APIName, a.Id, "ChannelNumber", a.ChannelNumber));
            }
        }

        if (ret.Count > 0)
        {
            await hubContext.Clients.All.SetField(ret).ConfigureAwait(false);
        }

        //await hubContext.Clients.All.DataRefresh("StreamGroups").ConfigureAwait(false);

        // await hubContext.Clients.All.DataRefresh("StreamGroupSMChannelLinks").ConfigureAwait(false);

        return APIResponse.Success;
    }
}
