using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupChannelGroups.Commands;

namespace StreamMasterApplication.StreamGroupChannelGroups;

public interface IStreamGroupChannelGroupController
{
    Task<ActionResult<StreamGroupDto?>> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);    
}

public interface IStreamGroupChannelGroupHub
{
    Task SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);

}