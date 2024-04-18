using StreamMaster.Application.StreamGroups.Events;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.StreamGroups.Commands;

[LogExecutionTimeAspect]
public class UpdateStreamGroupRequestHandler(IMessageService messageService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<UpdateStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.Name))
        {
            return APIResponse.NotFound;
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroup(request);
        if (streamGroup is not null)
        {

            await Publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
        }

        return APIResponse.Ok;
    }
}
