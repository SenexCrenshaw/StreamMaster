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

        StreamGroup? streamGroup = Repository.StreamGroup.GetStreamGroup(request.StreamGroupId);
        if (streamGroup != null)
        {
            //DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList()), cancellationToken);

            //GetSMChannelStreamsRequest re = new(request.SMChannelId);
            //FieldData fd = new("GetSMChannelStreams", re, streams.Data);

            //await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        }

        return res;
    }
}
