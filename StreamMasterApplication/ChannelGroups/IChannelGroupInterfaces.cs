using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.ChannelGroups.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.ChannelGroups;

public interface IChannelGroupController
{
    Task<ActionResult> AddChannelGroup(AddChannelGroupRequest request);

    Task<ActionResult> DeleteChannelGroup(DeleteChannelGroupRequest request);

    Task<ActionResult<ChannelGroupDto>> GetChannelGroup(int id);

    Task<ActionResult<IEnumerable<ChannelGroupDto>>> GetChannelGroups();

    Task<ActionResult> SetChannelGroupsVisible(SetChannelGroupsVisibleRequest request);

    Task<ActionResult> UpdateChannelGroup(UpdateChannelGroupRequest request);

    Task<ActionResult> UpdateChannelGroupOrder(UpdateChannelGroupOrderRequest request);

    Task<ActionResult> UpdateChannelGroups(UpdateChannelGroupsRequest request);
}

public interface IChannelGroupDB
{
    DbSet<ChannelGroup> ChannelGroups { get; set; }
}

public interface IChannelGroupHub
{
    Task<ChannelGroupDto?> AddChannelGroup(AddChannelGroupRequest request);

    Task<int?> DeleteChannelGroup(DeleteChannelGroupRequest request);

    Task<ChannelGroupDto?> GetChannelGroup(int id);

    Task<IEnumerable<ChannelGroupDto>?> GetChannelGroups();

    Task<IEnumerable<SetChannelGroupsVisibleArg>> SetChannelGroupsVisible(SetChannelGroupsVisibleRequest request);

    Task<ChannelGroupDto?> UpdateChannelGroup(UpdateChannelGroupRequest request);

    Task<IEnumerable<ChannelGroupDto>?> UpdateChannelGroupOrder(UpdateChannelGroupOrderRequest request);

    Task<IEnumerable<ChannelGroupDto>?> UpdateChannelGroups(UpdateChannelGroupsRequest request);
}

public interface IChannelGroupScoped
{
}

public interface IChannelGroupTasks
{
}
