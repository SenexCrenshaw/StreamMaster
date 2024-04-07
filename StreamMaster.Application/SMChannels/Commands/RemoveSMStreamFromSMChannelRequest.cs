using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveSMStreamFromSMChannelRequest(int SMChannelId, string SMStreamId) : IRequest<APIResponse>;

internal class RemoveSMStreamFromSMChannelRequestHandler(IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<RemoveSMStreamFromSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveSMStreamFromSMChannelRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.RemoveSMStreamFromSMChannel(request.SMChannelId, request.SMStreamId).ConfigureAwait(false);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        SMChannel? channel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (channel != null)
        {
            DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList()), cancellationToken);
            FieldData fd = new(nameof(SMChannelDto), channel.Id.ToString(), nameof(SMChannelDto.SMStreams), streams.Data);

            await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        }

        return res;
    }
}
