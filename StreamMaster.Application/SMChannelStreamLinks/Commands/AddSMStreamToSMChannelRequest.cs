using Microsoft.AspNetCore.Http;

using StreamMaster.Application.SMChannelStreamLinks.Queries;

namespace StreamMaster.Application.SMChannelStreamLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddSMStreamToSMChannelRequest(int SMChannelId, string SMStreamId) : IRequest<APIResponse>;


internal class AddSMStreamToSMChannelRequestHandler(IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<AddSMStreamToSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddSMStreamToSMChannelRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.AddSMStreamToSMChannel(request.SMChannelId, request.SMStreamId).ConfigureAwait(false);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        SMChannel? channel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (channel != null)
        {
            DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList()), cancellationToken);

            GetSMChannelStreamsRequest re = new(request.SMChannelId);
            FieldData fd = new("GetSMChannelStreams", re, streams.Data);

            await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        }

        return res;
    }
}
