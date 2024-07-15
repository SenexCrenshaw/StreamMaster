namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelFromStreamsRequest(List<string> StreamIds, int? StreamGroupId, int? M3UFileId) : IRequest<APIResponse>;

internal class CreateSMChannelFromStreamsRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<CreateSMChannelFromStreamsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelFromStreamsRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelFromStreams(request.StreamIds, request.StreamGroupId, request.M3UFileId);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();


        return APIResponse.Success;
    }
}
