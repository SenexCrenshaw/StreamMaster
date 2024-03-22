namespace StreamMaster.Domain.Repository;

public interface IStreamGroupChannelGroupRepository : IRepositoryBase<StreamGroupChannelGroup>
{
    Task AddVideoStreamtsToStreamGroup(int StreamGroupId, List<int> cgsToAdd);
    Task<List<string>> RemoveStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds);
    Task<StreamGroupDto?> SyncStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds);
    Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroups(List<int> channelGroupIds);
    Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroup(int channelGroupId);
    Task<List<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(int streamGroupId);
    Task<List<StreamGroupDto>?> SyncStreamGroupChannelGroupByChannelId(int ChannelGroupId);
}
