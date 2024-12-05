using StreamMaster.Application.SMChannelStreamLinks.Queries;

namespace StreamMaster.Application.SMChannelStreamLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMStreamRanksRequest(List<SMChannelStreamRankRequest> Requests) : IRequest<APIResponse>;

internal class SetSMStreamRanksRequestHandler(IRepositoryWrapper Repository, ISender Sender, IDataRefreshService dataRefreshService)
    : IRequestHandler<SetSMStreamRanksRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMStreamRanksRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannelStreamLink.SetSMStreamRank(request.Requests);
        if (!ret.IsError)
        {
            List<FieldData> fieldDatas = [];
            foreach (int smChannelId in request.Requests.Select(a => a.SMChannelId).Distinct())
            {
                SMChannel? smChannel = Repository.SMChannel.GetSMChannel(smChannelId);
                if (smChannel != null)
                {
                    DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(smChannel.Id, smChannel.SMStreams.Select(a => a.SMStream.Id).ToList()), cancellationToken);

                    if (!fieldDatas.Any(a => a.Entity == "GetSMChannelStreams"))
                    {
                        GetSMChannelStreamsRequest re = new(smChannel.Id);

                        fieldDatas.Add(new("GetSMChannelStreams", re, streams.Data ?? []));
                        fieldDatas.Add(new(SMChannel.APIName, smChannel.Id, "SMStreamDtos", streams.Data ?? []));
                    }
                }
            }
            await dataRefreshService.RefreshSMChannelStreamLinks();
            await dataRefreshService.SetField(fieldDatas).ConfigureAwait(false);
        }
        return ret;
    }
}
