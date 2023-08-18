using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IChannelGroupRepository
    {
        Task ChannelGroupCreateEmptyCount(int Id);
        Task ChannelGroupRemoveCount(int Id);

        Task ChannelGroupDeleteFromCount(int Id, int toDelete, bool isHidden, bool ignoreSave = false);
        Task ChannelGroupDeleteFromCounts(List<int> Ids, int toDelete, bool isHidden);

        Task ChannelGroupAddToCount(int Id, int toAdd, bool isHidden, bool ignoreSave = false);
        Task ChannelGroupsAddToCount(List<int> Ids, int toAdd, bool isHidden);

        Task ChannelGroupsSetCount(List<int> Ids, int toAdd);
        Task ChannelGroupSetCount(int Id, int toAdd, bool ignoreSave = false);

        Task<List<int>> GetChannelIdsFromVideoStream(VideoStreamDto videoStreamDto);
        Task AddOrUpdateChannelGroupVideoStreamCounts(List<ChannelGroupStreamCount> channelGroupStreamCounts);
        ChannelGroupStreamCount? GetChannelGroupVideoStreamCount(int id);
        IEnumerable<ChannelGroupStreamCount> GetChannelGroupVideoStreamCounts();
        Task AddOrUpdateChannelGroupVideoStreamCount(ChannelGroupStreamCount response, bool ignoreSave = false);
        IQueryable<string> GetAllChannelGroupNames();
        IQueryable<ChannelGroup> GetAllChannelGroups();

        Task<ChannelGroup?> GetChannelGroupByNameAsync(string name);

        Task<ChannelGroupDto?> GetChannelGroupAsync(int Id, CancellationToken cancellationToken = default);

        Task<PagedResponse<ChannelGroupDto>> GetChannelGroupsAsync(ChannelGroupParameters channelGroupParameters);

        void CreateChannelGroup(ChannelGroup ChannelGroup);

        void DeleteChannelGroup(ChannelGroup ChannelGroup);

        void UpdateChannelGroup(ChannelGroup ChannelGroup);
        Task<List<string>> GetChannelNamesFromVideoStream(VideoStreamDto videoStreamDto, CancellationToken cancellationToken);
        //Task<(ChannelGroupDto? channelGroup, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroupIds)> UpdateChannelGroup(UpdateChannelGroupRequest request, string url, CancellationToken cancellationToken);
    }
}