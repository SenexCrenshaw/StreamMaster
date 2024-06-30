using StreamMaster.Application.SMChannelStreamLinks.Queries;

namespace StreamMaster.Application.SMChannelStreamLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveSMStreamFromSMChannelRequest(int SMChannelId, string SMStreamId) : IRequest<APIResponse>;

internal class RemoveSMStreamFromSMChannelRequestHandler(IRepositoryWrapper Repository, ISender Sender, IDataRefreshService dataRefreshService)
    : IRequestHandler<RemoveSMStreamFromSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveSMStreamFromSMChannelRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.RemoveSMStreamFromSMChannel(request.SMChannelId, request.SMStreamId).ConfigureAwait(false);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (smChannel != null)
        {
            DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(smChannel.Id, smChannel.SMStreams.Select(a => a.SMStream).ToList()), cancellationToken);

            GetSMChannelStreamsRequest re = new(request.SMChannelId);
            List<FieldData> ret = new()
            {
                new("GetSMChannelStreams", re, streams.Data),
                new(SMChannel.APIName, smChannel.Id, "SMStreams", streams.Data)
            };

            //await dataRefreshService.RefreshSMChannelStreamLinks();
            await dataRefreshService.SetField(ret).ConfigureAwait(false);
        }

        return res;
    }
}
