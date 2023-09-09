using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.StreamGroupChannelGroups.Commands;
using StreamMasterApplication.StreamGroupChannelGroups.Queries;

namespace StreamMasterApplication.StreamGroupChannelGroups;

public interface IStreamGroupChannelGroupController
{
    Task<ActionResult<List<ChannelGroupDto>>> GetAllChannelGroups(GetAllChannelGroupsRequest request);
    Task<ActionResult<StreamGroupDto?>> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
    //Task<ActionResult<IEnumerable<ChannelGroupDto>>> GetChannelGroupsFromStreamGroup(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken);
}

public interface IStreamGroupChannelGroupHub
{
    Task<List<ChannelGroupDto>> GetAllChannelGroups(GetAllChannelGroupsRequest request);
    Task SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
    //Task<IEnumerable<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken);
}