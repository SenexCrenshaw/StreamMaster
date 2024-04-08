using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteSMChannelRequest(int SMChannelId) : IRequest<APIResponse>;

internal class DeleteSMChannelRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteSMChannelRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.DeleteSMChannel(request.SMChannelId);
        if (ret.IsError)
        {
            await messageService.SendError($"Could not delete channel", ret.ErrorMessage);
        }
        else
        {
            await Repository.SMChannelStreamLink.DeleteSMChannelStreamLinksFromParentId(request.SMChannelId);
            await hubContext.Clients.All.DataRefresh("GetPagedSMChannels").ConfigureAwait(false);
            await messageService.SendInfo($"Deleted channel {ret.Message}");
        }
        return ret;
    }
}
