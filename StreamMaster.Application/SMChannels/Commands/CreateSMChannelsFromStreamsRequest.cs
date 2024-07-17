namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelsFromStreamsRequest(List<string> StreamIds, string? StreamGroup, int? M3UFileId) : IRequest<APIResponse>;

internal class CreateSMChannelsFromStreamsRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<CreateSMChannelsFromStreamsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelsFromStreamsRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelsFromStreams(request.StreamIds, request.M3UFileId);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();


        return APIResponse.Success;
    }
}
