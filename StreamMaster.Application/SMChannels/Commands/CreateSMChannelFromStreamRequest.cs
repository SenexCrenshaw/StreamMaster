using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record CreateSMChannelFromStreamRequest(string streamId) : IRequest<DefaultAPIResponse>;

internal class CreateSMChannelFromStreamRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateSMChannelFromStreamRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(CreateSMChannelFromStreamRequest request, CancellationToken cancellationToken)
    {
        await Repository.SMChannel.CreateSMChannelFromStream(request.streamId);

        await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);

        return APIResponseFactory.Ok;
    }
}
