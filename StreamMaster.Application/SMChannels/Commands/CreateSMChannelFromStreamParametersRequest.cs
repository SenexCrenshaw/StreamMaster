namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelFromStreamParametersRequest(QueryStringParameters Parameters, int? StreamGroupId, int? M3UFileId) : IRequest<APIResponse>;

internal class CreateSMChannelFromStreamParametersRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<CreateSMChannelFromStreamParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelFromStreamParametersRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelFromStreamParameters(request.Parameters, request.StreamGroupId, request.M3UFileId);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();


        return APIResponse.Success;
    }
}
