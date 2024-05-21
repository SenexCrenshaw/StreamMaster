namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMChannelVisibleByIdRequest(int Id) : IRequest<APIResponse>;
internal class ToggleSMChannelVisibleByIdHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<ToggleSMChannelVisibleByIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMChannelVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        SMChannelDto? channel = await Repository.SMChannel.ToggleSMChannelVisibleById(request.Id, cancellationToken).ConfigureAwait(false);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        FieldData fd = new(SMStream.APIName, channel.Id, "IsHidden", channel.IsHidden);

        await dataRefreshService.SetField([fd]).ConfigureAwait(false);
        await dataRefreshService.ClearByTag(SMChannel.APIName, "IsHidden").ConfigureAwait(false);

        return APIResponse.Success;
    }
}
