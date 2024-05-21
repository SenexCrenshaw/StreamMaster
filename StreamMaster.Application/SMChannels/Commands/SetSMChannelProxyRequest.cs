namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelProxyRequest(int SMChannelId, int StreamingProxy) : IRequest<APIResponse>;

internal class SetSMChannelProxyRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelProxyRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelProxyRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelProxy(request.SMChannelId, request.StreamingProxy).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set EPG failed {ret.Message}");
            return ret;
        }

        //if (smChannel.StreamingProxyType != request.StreamingProxyType)
        //{
        //    smChannel.StreamingProxyType = request.StreamingProxyType.Value;
        //    ret.Add(new FieldData(() => smChannel.StreamingProxyType));
        //}


        FieldData fd = new(SMChannel.APIName, request.SMChannelId, "StreamingProxyType", request.StreamingProxy);
        await dataRefreshService.SetField([fd]).ConfigureAwait(false);

        return ret;
    }
}
