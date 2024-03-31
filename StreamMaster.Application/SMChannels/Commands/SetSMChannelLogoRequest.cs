namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelLogoRequest(int SMChannelId, string logo) : IRequest<DefaultAPIResponse>;

internal class SetSMChannelLogoRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SetSMChannelLogoRequest, DefaultAPIResponse>
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
