namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelFromStreamParametersRequest(QueryStringParameters Parameters, int? StreamGroupId) : IRequest<APIResponse>;

internal class CreateSMChannelFromStreamParametersRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<CreateSMChannelFromStreamParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelFromStreamParametersRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelFromStreamParameters(request.Parameters, request.StreamGroupId);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();


        return APIResponse.Success;
    }
}
