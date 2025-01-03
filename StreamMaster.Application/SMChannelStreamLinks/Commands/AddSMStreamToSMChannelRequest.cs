namespace StreamMaster.Application.SMChannelStreamLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddSMStreamToSMChannelRequest(int SMChannelId, string SMStreamId, int? Rank) : IRequest<APIResponse>;

internal class AddSMStreamToSMChannelRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<AddSMStreamToSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddSMStreamToSMChannelRequest request, CancellationToken cancellationToken)
    {
        SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (smChannel == null)
        {
            return APIResponse.ErrorWithMessage($"Channel with Id {request.SMChannelId} or stream with Id {request.SMStreamId} not found");
        }

        await Repository.SMChannelStreamLink.CreateSMChannelStreamLink(smChannel.Id, request.SMStreamId, request.Rank);
        await Repository.SaveAsync();
        //DataResponse<List<SMStreamDto>> streams = await Sender.Send(new UpdateStreamRanksRequest(SMChannel.Id, SMChannel.SMStreams.Select(a => a.SMStream.Id).ToList()), cancellationToken);

        //GetSMChannelStreamsRequest re = new(request.Id);

        //List<SMStreamDto> dtos = Mapper.Map<List<SMStreamDto>>(SMChannel.SMStreams.Select(a => a.SMStream));

        //List<FieldData> ret =
        //[
        //    new("GetSMChannelStreams", re, streams.Data),
        //    new(SMChannel.APIName, SMChannel.Id, "SMStreams",streams.Data)
        //];
        await dataRefreshService.RefreshSMChannelStreamLinks();
        await dataRefreshService.RefreshSMChannels();
        await dataRefreshService.RefreshSMStreams();
        //await dataRefreshService.SetField(ret).ConfigureAwait(false);

        return APIResponse.Success;
    }
}
