namespace StreamMaster.Application.SMChannelStreamLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveSMStreamFromSMChannelRequest(int SMChannelId, string SMStreamId) : IRequest<APIResponse>;

internal class RemoveSMStreamFromSMChannelRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<RemoveSMStreamFromSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveSMStreamFromSMChannelRequest request, CancellationToken cancellationToken)
    {
        SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.SMChannelId);
        if (smChannel == null)
        {
            return APIResponse.ErrorWithMessage($"Channel with Id {request.SMChannelId} not found");
        }
        IQueryable<SMChannelStreamLink> toDelete = Repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == request.SMChannelId && a.SMStreamId == request.SMStreamId);

        if (!toDelete.Any())
        {
            return APIResponse.Ok;
        }

        await Repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(toDelete);

        await dataRefreshService.RefreshSMChannelStreamLinks();
        await dataRefreshService.RefreshSMChannels();
        await dataRefreshService.RefreshSMStreams();

        return APIResponse.Success;
    }
}
