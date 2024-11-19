using StreamMaster.Application.SMChannelStreamLinks.Queries;

namespace StreamMaster.Application.SMChannelChannelLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelRanksRequest(List<SMChannelChannelRankRequest> Requests) : IRequest<APIResponse>;

internal class SetSMChannelRanksRequestHandler(IRepositoryWrapper Repository, ISender Sender, IDataRefreshService dataRefreshService)
    : IRequestHandler<SetSMChannelRanksRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelRanksRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannelChannelLink.SetSMChannelRanks(request.Requests).ConfigureAwait(false);
        if (!ret.IsError)
        {
            List<FieldData> fieldDatas = [];
            foreach (int smChannelId in request.Requests.Select(a => a.ParentSMChannelId).Distinct())
            {
                SMChannel? smChannel = Repository.SMChannel.GetSMChannel(smChannelId);
                if (smChannel != null)
                {
                    DataResponse<List<SMChannelDto>> channels = await Sender.Send(new UpdateSMChannelRanksRequest(smChannel.Id, smChannel.SMChannels.Select(a => a.SMChannelId).ToList()), cancellationToken);

                    if (!fieldDatas.Any(a => a.Entity == "GetSMChannelChannels"))
                    {
                        GetSMChannelStreamsRequest re = new(smChannel.Id);

                        fieldDatas.Add(new("GetSMChannelChannels", re, channels.Data ?? []));
                        fieldDatas.Add(new(SMChannel.APIName, smChannel.Id, "SMChannels", channels.Data ?? []));
                    }

                }

            }
            await dataRefreshService.SetField(fieldDatas).ConfigureAwait(false);
        }
        return ret;
    }
}
