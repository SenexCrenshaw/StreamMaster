namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CopySMChannelRequest(int SMChannelId, string NewName) : IRequest<APIResponse>;

internal class CopySMChannelRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<CopySMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CopySMChannelRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.CloneSMChannel(request.SMChannelId, request.NewName);
        if (ret.IsError)
        {
            await messageService.SendError($"Could not copy channel", ret.ErrorMessage);
        }
        else
        {
            await dataRefreshService.RefreshAllSMChannels();
            await messageService.SendSuccess($"Copied channel");
        }
        return ret;
    }
}
