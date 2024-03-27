using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record RemoveSMStreamFromSMChannelRequest(int SMChannelId, string SMStreamId) : IRequest<DefaultAPIResponse>;

internal class RemoveSMStreamFromSMChannelRequestHandler(IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<RemoveSMStreamFromSMChannelRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(RemoveSMStreamFromSMChannelRequest request, CancellationToken cancellationToken)
    {
        DefaultAPIResponse ret = await Repository.SMChannel.RemoveSMStreamFromSMChannel(request.SMChannelId, request.SMStreamId).ConfigureAwait(false);
        if (!ret.IsError.HasValue)
        {
            SMChannel? channel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
            if (channel != null)
            {
                List<SMStreamDto> streams = await Sender.Send(new UpdateStreamRanksRequest(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList()));
                FieldData fd = new(nameof(SMChannelDto), channel.Id.ToString(), "smStreams", streams);

                await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
            }
        }
        return ret;
    }
}
