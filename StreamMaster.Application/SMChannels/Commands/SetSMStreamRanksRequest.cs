using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
public record SetSMStreamRanksRequest(List<SMChannelRankRequest> requests) : IRequest<DefaultAPIResponse>;

internal class SetSMStreamRanksRequestHandler(IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<SetSMStreamRanksRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SetSMStreamRanksRequest request, CancellationToken cancellationToken)
    {
        DefaultAPIResponse ret = await Repository.SMChannel.SetSMStreamRanks(request.requests).ConfigureAwait(false);
        if (!ret.IsError.HasValue)
        {
            List<FieldData> fieldDatas = [];
            foreach (int smChannelId in request.requests.Select(a => a.SMChannelId).Distinct())
            {
                SMChannel? channel = Repository.SMChannel.GetSMChannel(smChannelId);
                if (channel != null)
                {
                    List<SMStreamDto> streams = await Sender.Send(new UpdateStreamRanksRequest(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList()));
                    FieldData fd = new(nameof(SMChannelDto), channel.Id.ToString(), "smStreams", streams);
                    fieldDatas.Add(fd);
                }

            }
            await hubContext.Clients.All.SetField(fieldDatas.ToArray()).ConfigureAwait(false);
        }
        return ret;
    }
}
