namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AutoSetSMChannelNumbersRequest(int streamGroupId, int startingNumber, bool overWriteExisting, QueryStringParameters Parameters) : IRequest<APIResponse>;

internal class AutoSetSMChannelNumbersRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<AutoSetSMChannelNumbersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AutoSetSMChannelNumbersRequest request, CancellationToken cancellationToken)
    {
        var res = await Repository.StreamGroup.AutoSetSMChannelNumbers(request.streamGroupId, request.startingNumber, request.overWriteExisting, request.Parameters);
        if (res.APIResponse.IsError)
        {
            return APIResponse.ErrorWithMessage(res.APIResponse.ErrorMessage);
        }
        List<FieldData> ret = [];
        foreach (var item in res)
        {
            var a = item.Result as SMChannel;
            if (a != null)
            {
                ret.Add(new FieldData(SMChannel.MainGet, a.Id.ToString(), "ChannelNumber", a.ChannelNumber));
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
