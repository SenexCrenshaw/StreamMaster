namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateChannelGroupRequest(string GroupName, bool IsReadOnly) : IRequest<APIResponse>;

public class CreateChannelGroupRequestHandler(IMessageService messageService, IDataRefreshService dataRefreshService, IRepositoryWrapper Repository)
    : IRequestHandler<CreateChannelGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateChannelGroupRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.GroupName))
        {
            return APIResponse.ErrorWithMessage("Group Name is required");
        }
        if (request.GroupName.Equals("dummy", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"Group Name cannot be '{request.GroupName}'");
        }

        if (Repository.ChannelGroup.Any(a => a.Name == request.GroupName))
        {
            return APIResponse.Ok;
        }

        APIResponse res = await Repository.ChannelGroup.CreateChannelGroup(request.GroupName, request.IsReadOnly);
        if (res.IsError)
        {
            return res;
        }

        _ = await Repository.SaveAsync().ConfigureAwait(false);

        await dataRefreshService.RefreshChannelGroups().ConfigureAwait(false);

        await messageService.SendSuccess($"Created Channel Group '{request.GroupName}'");
        return APIResponse.Success;
    }
}
