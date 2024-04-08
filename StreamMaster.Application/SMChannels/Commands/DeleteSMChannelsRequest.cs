using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteSMChannelsRequest(List<int> SMChannelIds) : IRequest<APIResponse>;

internal class DeleteSMChannelsRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteSMChannelsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteSMChannelsRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.DeleteSMChannels(request.SMChannelIds);
        if (!ret.IsError)
        {
            await hubContext.Clients.All.DataRefresh("GetPagedSMChannels").ConfigureAwait(false);
        }

        return ret;
    }
}
