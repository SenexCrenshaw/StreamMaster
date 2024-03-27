using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record SetSMChannelLogoRequest(int SMChannelId, string logo) : IRequest<DefaultAPIResponse>;

internal class SetSMChannelLogoRequestHandler(IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<SetSMChannelLogoRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SetSMChannelLogoRequest request, CancellationToken cancellationToken)
    {
        string? logo = await Repository.SMChannel.SetSMChannelLogo(request.SMChannelId, request.logo).ConfigureAwait(false);
        if (logo == null)
        {
            return APIResponseFactory.NotFound;
        }

        FieldData fd = new(nameof(SMChannelDto), request.SMChannelId.ToString(), "logo", logo);

        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        return APIResponseFactory.Ok;
    }
}
