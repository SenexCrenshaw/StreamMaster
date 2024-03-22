using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record CreateSMChannelFromStream(string streamId) : IRequest<DefaultAPIResponse>;

internal class CreateSMChannelFromStreamRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateSMChannelFromStream, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(CreateSMChannelFromStream request, CancellationToken cancellationToken)
    {
        await Repository.SMChannel.CreateSMChannelFromStream(request.streamId);

        await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);

        return APIResponseFactory.Ok;
    }
}
