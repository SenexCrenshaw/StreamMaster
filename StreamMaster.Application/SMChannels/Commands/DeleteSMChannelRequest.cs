using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record DeleteSMChannelRequest(int smChannelId) : IRequest<DefaultAPIResponse>;

internal class DeleteSMChannelRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<DeleteSMChannelRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(DeleteSMChannelRequest request, CancellationToken cancellationToken)
    {
        DefaultAPIResponse ret = await Repository.SMChannel.DeleteSMChannel(request.smChannelId);
        if (ret.IsError.HasValue && ret.IsError.Value)
        {

            await messageService.SendError($"Could not delete channel", ret.ErrorMessage);
        }
        else
        {
            await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);
            await messageService.SendSMInfo($"Deleted channel {ret.Message}");
        }
        return ret;
    }
}
