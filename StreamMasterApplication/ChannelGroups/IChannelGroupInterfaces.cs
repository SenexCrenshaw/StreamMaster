using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.ChannelGroups.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups;

public interface IChannelGroupController
{
    Task<ActionResult> CreateChannelGroup(CreateChannelGroupRequest request);

    Task<ActionResult> DeleteChannelGroup(DeleteChannelGroupRequest request);

    Task<ActionResult<ChannelGroupDto>> GetChannelGroup(int id);


    Task<ActionResult<IEnumerable<ChannelGroupDto>>> GetChannelGroups(ChannelGroupParameters channelGroupParameters);
    Task<ActionResult> SetChannelGroupsVisible(SetChannelGroupsVisibleRequest request);

    Task<ActionResult> UpdateChannelGroup(UpdateChannelGroupRequest request);


    Task<ActionResult> UpdateChannelGroups(UpdateChannelGroupsRequest request);
}

public interface IChannelGroupDB
{
    //DbSet<ChannelGroup> ChannelGroups { get; set; }

    //Task<(ChannelGroupDto? channelGroup, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroupIds)> UpdateChannelGroup(UpdateChannelGroupRequest request, string url, CancellationToken cancellationToken);
}

public interface IChannelGroupHub
{
    Task CreateChannelGroup(CreateChannelGroupRequest request);

    Task DeleteChannelGroup(DeleteChannelGroupRequest request);

    Task<ChannelGroupDto?> GetChannelGroup(int id);
    Task<PagedList<ChannelGroup>> GetChannelGroups(ChannelGroupParameters channelGroupParameters);

    Task SetChannelGroupsVisible(SetChannelGroupsVisibleRequest request);

    Task UpdateChannelGroup(UpdateChannelGroupRequest request);


    Task UpdateChannelGroups(UpdateChannelGroupsRequest request);
}

public interface IChannelGroupScoped
{
}

public interface IChannelGroupTasks
{
}
