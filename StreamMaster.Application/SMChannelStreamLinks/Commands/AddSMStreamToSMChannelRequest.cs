namespace StreamMaster.Application.SMChannelStreamLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddSMStreamToSMChannelRequest(int SMChannelId, string SMStreamId, int? Rank) : IRequest<APIResponse>;


internal class AddSMStreamToSMChannelRequestHandler(IRepositoryWrapper Repository, ISender Sender, IMapper Mapper, IDataRefreshService dataRefreshService)
    : IRequestHandler<AddSMStreamToSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddSMStreamToSMChannelRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.AddSMStreamToSMChannel(request.SMChannelId, request.SMStreamId, request.Rank).ConfigureAwait(false);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (smChannel != null)
        {
            //smChannel.SMStreams.Add
            await Repository.SMChannelStreamLink.CreateSMChannelStreamLink(smChannel.Id, request.SMStreamId, null);
            await Repository.SaveAsync();
            //DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(smChannel.Id, smChannel.SMStreams.Select(a => a.SMStream.Id).ToList()), cancellationToken);

            //GetSMChannelStreamsRequest re = new(request.Id);

            //List<SMStreamDto> dtos = Mapper.Map<List<SMStreamDto>>(smChannel.SMStreams.Select(a => a.SMStream));

            //List<FieldData> ret =
            //[
            //    new("GetSMChannelStreams", re, streams.Data),
            //    new(SMChannel.APIName, smChannel.Id, "SMStreams",streams.Data)
            //];
            await dataRefreshService.RefreshSMChannelStreamLinks();
            await dataRefreshService.RefreshSMChannels();
            await dataRefreshService.RefreshSMStreams();
            //await dataRefreshService.SetField(ret).ConfigureAwait(false);

        }

        return res;
    }
}
