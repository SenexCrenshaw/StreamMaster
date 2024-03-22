using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.ChannelGroups.Commands;

namespace StreamMaster.Application.ChannelGroups
{
    public interface IChannelGroupsController
    {        
    Task<ActionResult<DefaultAPIResponse?>> CreateChannelGroup(CreateChannelGroupRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IChannelGroupsHub
    {
        Task<DefaultAPIResponse?> CreateChannelGroup(CreateChannelGroupRequest request);
    }
}
