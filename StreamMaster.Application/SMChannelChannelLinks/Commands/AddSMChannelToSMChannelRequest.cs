namespace StreamMaster.Application.SMChannelChannelLinks.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddSMChannelToSMChannelRequest(int ParentSMChannelId, int ChildSMChannelId, int? Rank) : IRequest<APIResponse>;

internal class AddSMChannelToSMChannelRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<AddSMChannelToSMChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddSMChannelToSMChannelRequest request, CancellationToken cancellationToken)
    {
        SMChannel? smChannel = Repository.SMChannel.GetSMChannel(request.ParentSMChannelId);
        if (smChannel == null)
        {
            return APIResponse.ErrorWithMessage($"Channel with Id {request.ParentSMChannelId} or channel with Id {request.ChildSMChannelId} not found");
        }

        await Repository.SMChannelChannelLink.CreateSMChannelChannelLink(smChannel.Id, request.ChildSMChannelId, null);
        await Repository.SaveAsync();

        await dataRefreshService.RefreshSMChannelStreamLinks();
        await dataRefreshService.RefreshSMChannels();
        await dataRefreshService.RefreshSMStreams();

        return APIResponse.Success;
    }
}
