using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteSMChannelsRequest(List<int> smChannelIds) : IRequest<DefaultAPIResponse>;

internal class DeleteSMChannelsRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteSMChannelsRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(DeleteSMChannelsRequest request, CancellationToken cancellationToken)
    {
        DefaultAPIResponse ret = await Repository.SMChannel.DeleteSMChannels(request.smChannelIds);
        if (!ret.IsError)
        {
            await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);
        }

        return ret;
    }
}
