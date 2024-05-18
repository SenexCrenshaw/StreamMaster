namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateChannelGroupsRequest(List<UpdateChannelGroupRequest> requests)
    : IRequest<APIResponse>
{ }

public class UpdateChannelGroupsRequestHandler(IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher, ISender Sender)
    : IRequestHandler<UpdateChannelGroupsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateChannelGroupsRequest request, CancellationToken cancellationToken)
    {

        foreach (UpdateChannelGroupRequest clientRequest in request.requests)
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
