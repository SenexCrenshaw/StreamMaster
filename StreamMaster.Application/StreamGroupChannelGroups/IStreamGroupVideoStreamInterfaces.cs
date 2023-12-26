using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Dto;

using StreamMaster.Application.StreamGroupChannelGroups.Commands;

namespace StreamMaster.Application.StreamGroupChannelGroups;

public interface IStreamGroupChannelGroupController
{
    Task<ActionResult<StreamGroupDto?>> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);    
}

public interface IStreamGroupChannelGroupHub
{
    Task SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);

}