using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Repository;

public interface IStreamGroupChannelGroupRepository : IRepositoryBase<StreamGroupChannelGroup>
{
    Task<IEnumerable<string>> RemoveStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default);
    Task<int> SyncStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default);
    Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroups(List<int> channelGroupIds, CancellationToken cancellationToken = default);
    Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroup(int channelGroupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(int streamGroupId, CancellationToken cancellationToken);
}
