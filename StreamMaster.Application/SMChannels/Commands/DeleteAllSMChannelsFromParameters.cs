using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record DeleteAllSMChannelsFromParameters(SMChannelParameters Parameters) : IRequest<DefaultAPIResponse>;

internal class DeleteAllSMChannelsFromParametersRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteAllSMChannelsFromParameters, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(DeleteAllSMChannelsFromParameters request, CancellationToken cancellationToken)
    {
        List<int> ids = await Repository.SMChannel.DeleteAllSMChannelsFromParameters(request.Parameters).ConfigureAwait(false);

        if (ids.Count != 0)
        {
            await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);
        }

        return APIResponseFactory.Ok;
    }
}
