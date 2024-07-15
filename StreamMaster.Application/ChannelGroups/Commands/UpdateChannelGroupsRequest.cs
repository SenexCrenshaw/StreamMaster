namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateChannelGroupsRequest(List<UpdateChannelGroupRequest> UpdateChannelGroupRequests)
    : IRequest<APIResponse>;

public class UpdateChannelGroupsRequestHandler(ISender Sender)
    : IRequestHandler<UpdateChannelGroupsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        foreach (UpdateChannelGroupRequest clientRequest in request.UpdateChannelGroupRequests)
        {
            APIResponse res = await Sender.Send(clientRequest, cancellationToken).ConfigureAwait(false);
            if (res.IsError)
            {
                return APIResponse.ErrorWithMessage($"Updating Channel Group {clientRequest.ChannelGroupId} failed");
            }
        }

        return APIResponse.Success;
    }
}
