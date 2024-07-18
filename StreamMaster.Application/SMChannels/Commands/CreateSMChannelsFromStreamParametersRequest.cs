﻿namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelsFromStreamParametersRequest(QueryStringParameters Parameters, string? DefaultStreamGroupName, int? StreamGroupId, int? M3UFileId) : IRequest<APIResponse>;

internal class CreateSMChannelsFromStreamParametersRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<CreateSMChannelsFromStreamParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelsFromStreamParametersRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelsFromStreamParameters(request.Parameters, request.StreamGroupId, request.M3UFileId);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();


        return APIResponse.Success;
    }
}