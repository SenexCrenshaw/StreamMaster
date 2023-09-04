using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IChannelGroupRepository : IRepositoryBase<ChannelGroup>
    {
        Task<List<int>> DeleteAllChannelGroupsFromParameters(ChannelGroupParameters Parameters, CancellationToken cancellationToken);
        //Task AddOrUpdateChannelGroupVideoStreamCounts(List<ChannelGroupStreamCount> channelGroupStreamCounts);
        //ChannelGroupStreamCount? GetChannelGroupVideoStreamCount(int id);
        //IEnumerable<ChannelGroupStreamCount> GetChannelGroupVideoStreamCounts();
        //Task AddOrUpdateChannelGroupVideoStreamCount(ChannelGroupStreamCount response, bool ignoreSave = false);
        IQueryable<string> GetAllChannelGroupNames();
        Task<List<int>> GetChannelIdsFromVideoStream(VideoStreamDto videoStreamDto, CancellationToken cancellationToken);
        IQueryable<ChannelGroup> GetAllChannelGroups();
        Task<List<ChannelGroup>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames);
        Task<ChannelGroup?> GetChannelGroupByNameAsync(string name);

        Task<ChannelGroupDto?> GetChannelGroupAsync(int Id, CancellationToken cancellationToken = default);

        Task<ChannelGroup?> GetChannelGroupById(int Id);

        Task<PagedResponse<ChannelGroupDto>> GetChannelGroupsAsync(ChannelGroupParameters channelGroupParameters);

        void CreateChannelGroup(ChannelGroup ChannelGroup);

        void DeleteChannelGroup(ChannelGroup ChannelGroup);

        void UpdateChannelGroup(ChannelGroup ChannelGroup);
        Task<List<string>> GetChannelNamesFromVideoStream(VideoStreamDto videoStreamDto, CancellationToken cancellationToken);
        //Task<(ChannelGroupDto? channelGroup, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroupIds)> UpdateChannelGroup(UpdateChannelGroupRequest request, string url, CancellationToken cancellationToken);
    }
}