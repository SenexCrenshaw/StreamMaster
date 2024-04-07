using Microsoft.AspNetCore.Http;

namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMStreamRanksRequest(List<SMChannelRankRequest> Requests) : IRequest<APIResponse>;

internal class SetSMStreamRanksRequestHandler(IRepositoryWrapper Repository, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IOptionsMonitor<Setting> settings, IOptionsMonitor<HLSSettings> hlsSettings, IHttpContextAccessor httpContextAccessor) : IRequestHandler<SetSMStreamRanksRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMStreamRanksRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMStreamRanks(request.Requests).ConfigureAwait(false);
        if (!ret.IsError)
        {
            List<FieldData> fieldDatas = [];
            foreach (int smChannelId in request.Requests.Select(a => a.SMChannelId).Distinct())
            {
                SMChannel? channel = Repository.SMChannel.GetSMChannel(smChannelId);
                if (channel != null)
                {
                    DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(channel.Id, channel.SMStreams.Select(a => a.SMStream).ToList()), cancellationToken);
                    FieldData fd = new(nameof(SMChannelDto), channel.Id.ToString(), "SMStreams", streams.Data);
                    fieldDatas.Add(fd);
                }

            }
            await hubContext.Clients.All.SetField([.. fieldDatas]).ConfigureAwait(false);
        }
        return ret;
    }
}
