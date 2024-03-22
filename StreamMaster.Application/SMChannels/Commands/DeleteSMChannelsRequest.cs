using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record DeleteSMChannelsRequest(List<int> smChannelIds) : IRequest<DefaultAPIResponse>;

internal class DeleteSMChannelsRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteSMChannelsRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(DeleteSMChannelsRequest request, CancellationToken cancellationToken)
    {
        DefaultAPIResponse ret = await Repository.SMChannel.DeleteSMChannels(request.smChannelIds);
        if (!ret.IsError.HasValue)
        {
            await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);
        }

        return ret;
    }
}
