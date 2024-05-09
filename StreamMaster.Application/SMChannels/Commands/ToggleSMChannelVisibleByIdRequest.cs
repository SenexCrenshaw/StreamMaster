namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMChannelVisibleByIdRequest(int Id) : IRequest<APIResponse>;
internal class ToggleSMChannelVisibleByIdHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ToggleSMChannelVisibleByIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMChannelVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        SMChannelDto? channel = await Repository.SMChannel.ToggleSMChannelVisibleById(request.Id, cancellationToken).ConfigureAwait(false);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        FieldData fd = new(SMStream.MainGet, channel.Id.ToString(), "IsHidden", channel.IsHidden);

        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        await hubContext.Clients.All.ClearByTag(new ClearByTag("GetPagedSMChannels", "IsHidden")).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
