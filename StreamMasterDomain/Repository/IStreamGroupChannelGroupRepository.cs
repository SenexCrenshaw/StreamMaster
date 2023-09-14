﻿using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;

namespace StreamMasterDomain.Repository;

public interface IStreamGroupChannelGroupRepository : IRepositoryBase<StreamGroupChannelGroup>
{
    Task<List<string>> RemoveStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default);
    Task<StreamGroupDto?> SyncStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default);
    Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroups(List<int> channelGroupIds, CancellationToken cancellationToken = default);
    Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroup(int channelGroupId, CancellationToken cancellationToken = default);
    Task<List<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(int streamGroupId, CancellationToken cancellationToken);
}
