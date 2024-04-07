using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelFromStreamRequest(string StreamId) : IRequest<APIResponse>;

internal class CreateSMChannelFromStreamRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateSMChannelFromStreamRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelFromStreamRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelFromStream(request.StreamId);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await hubContext.Clients.All.DataRefresh("SMChannelDto").ConfigureAwait(false);

        return APIResponse.Success;
    }
}
