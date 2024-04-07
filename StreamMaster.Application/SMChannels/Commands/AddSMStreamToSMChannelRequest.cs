using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddSMStreamToSMChannelRequest(int SMChannelId, string SMStreamId) : IRequest<APIResponse>;


internal class AddSMStreamToSMChannelRequestHandler(IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<AddSMStreamToSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddSMStreamToSMChannelRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.AddSMStreamToSMChannel(request.SMChannelId, request.SMStreamId).ConfigureAwait(false);
        if (!ret.IsError)
        {
            SMChannel? channel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
            if (channel != null)
            {
                DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList()), cancellationToken);
                FieldData fd = new(nameof(SMChannelDto), channel.Id.ToString(), "SMStreams", streams.Data);
                await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
            }
        }
        return ret;
    }
}
