using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record DeleteSMChannel(int smChannelId) : IRequest<DefaultAPIResponse>;

internal class DeleteSMChannelRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteSMChannel, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(DeleteSMChannel request, CancellationToken cancellationToken)
    {
        SMChannel? channel = Repository.SMChannel.GetSMChannel(request.smChannelId);
        if (channel == null)
        {
            return APIResponseFactory.NotFound;
        }

        await Repository.SMChannel.DeleteSMChannel(request.smChannelId);

        await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);

        return APIResponseFactory.Ok;
    }
}
