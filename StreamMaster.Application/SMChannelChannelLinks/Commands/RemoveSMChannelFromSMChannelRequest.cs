namespace StreamMaster.Application.SMChannelChannelLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveSMChannelFromSMChannelRequest(int ParentSMChannelId, int ChildSMChannelId) : IRequest<APIResponse>;

internal class RemoveSMChannelFromSMChannelRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<RemoveSMChannelFromSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveSMChannelFromSMChannelRequest request, CancellationToken cancellationToken)
    {
        SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.ParentSMChannelId);
        if (smChannel == null)
        {
            return APIResponse.ErrorWithMessage($"Channel with Id {request.ParentSMChannelId} not found");
        }
        IQueryable<SMChannelChannelLink> toDelete = Repository.SMChannelChannelLink.GetQuery(true).Where(a => a.ParentSMChannelId == request.ParentSMChannelId && a.SMChannelId == request.ChildSMChannelId);

        if (!toDelete.Any())
        {
            return APIResponse.Ok;
        }

        await Repository.SMChannelChannelLink.DeleteSMChannelChannelLinks(toDelete);
        await dataRefreshService.RefreshSMChannelStreamLinks();
        await dataRefreshService.RefreshSMChannels();
        await dataRefreshService.RefreshSMStreams();
        return APIResponse.Success;
    }
}
