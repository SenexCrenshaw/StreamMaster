using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Dto;
using StreamMaster.Application.StreamGroupChannelGroupLinks.Commands;

namespace StreamMaster.Application.StreamGroupChannelGroupLinks;

public interface IStreamGroupChannelGroupController
{
    Task<ActionResult<StreamGroupDto?>> SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);
}

public interface IStreamGroupChannelGroupHub
{
    Task SyncStreamGroupChannelGroups(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken);

}