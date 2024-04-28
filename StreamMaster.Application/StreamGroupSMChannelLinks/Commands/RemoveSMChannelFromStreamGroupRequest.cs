using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.StreamGroupSMChannelLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveSMChannelFromStreamGroupRequest(int StreamGroupId, int SMChannelId) : IRequest<APIResponse>;

internal class RemoveSMChannelFromStreamGroupRequestHandler(IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<RemoveSMChannelFromStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveSMChannelFromStreamGroupRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.StreamGroupSMChannelLink.RemoveSMChannelFromStreamGroup(request.StreamGroupId, request.SMChannelId).ConfigureAwait(false);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await hubContext.Clients.All.DataRefresh("StreamGroupSMChannelLinks").ConfigureAwait(false);
        return res;
    }
}
