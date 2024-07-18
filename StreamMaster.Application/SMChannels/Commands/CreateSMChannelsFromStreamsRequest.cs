namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelsFromStreamsRequest(List<string> StreamIds, int? M3UFileId, int? StreamGroupId = null, bool? IsCustomPlayList = false) : IRequest<APIResponse>;

internal class CreateSMChannelsFromStreamsRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<CreateSMChannelsFromStreamsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelsFromStreamsRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelsFromStreams(request.StreamIds, AddToStreamGroupId: request.StreamGroupId, M3UFileId: request.M3UFileId, IsCustomPlayList: request.IsCustomPlayList);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();


        return APIResponse.Success;
    }
}
