namespace StreamMaster.Application.Interfaces
{
    public interface IChannelGroupService
    {
        Task<ChannelGroupDto> UpdateChannelGroupCountRequestAsync(ChannelGroupDto ChannelGroup);
        Task UpdateChannelGroupCountsRequestAsync(List<ChannelGroup>? ChannelGroups = null);
    }
}