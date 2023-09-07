using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupChannelGroups.Commands;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

namespace StreamMasterApplication.StreamGroupChannelGroups;

public interface IStreamGroupChannelGroupController
{
    Task<ActionResult<StreamGroupDto?>> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
    Task<ActionResult<IEnumerable<ChannelGroupDto>>> GetChannelGroupsFromStreamGroup(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken);
}

public interface IStreamGroupChannelGroupHub
{
    Task<StreamGroupDto?> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
    Task<IEnumerable<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken);
}