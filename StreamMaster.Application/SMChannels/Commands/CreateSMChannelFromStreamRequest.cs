﻿namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelFromStreamRequest(string StreamId, int? StreamGroupId, int? M3UFileId) : IRequest<APIResponse>;

internal class CreateSMChannelFromStreamRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<CreateSMChannelFromStreamRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelFromStreamRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelFromStream(request.StreamId, request.StreamGroupId, request.M3UFileId);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();


        return APIResponse.Success;
    }
}