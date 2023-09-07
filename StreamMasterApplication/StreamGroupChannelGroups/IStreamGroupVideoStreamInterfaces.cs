using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupChannelGroups.Commands;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

namespace StreamMasterApplication.StreamGroupChannelGroups;

public interface IStreamGroupChannelGroupController
{
    Task<ActionResult<IEnumerable<string>>> RemoveStreamGroupChannelGroups(RemoveStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
    Task<ActionResult<int>> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
    Task<ActionResult<IEnumerable<ChannelGroupDto>>> GetChannelGroupsFromStreamGroup(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken);
}

public interface IStreamGroupChannelGroupHub
{
    Task<IEnumerable<string>> RemoveStreamGroupChannelGroups(RemoveStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
    Task<int> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
    Task<IEnumerable<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken);
}