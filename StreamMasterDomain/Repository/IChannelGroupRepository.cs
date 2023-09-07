using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IChannelGroupRepository : IRepositoryBase<ChannelGroup>
    {
        Task<(IEnumerable<int> ChannelGroupIds, IEnumerable<VideoStreamDto> VideoStreams)> DeleteAllChannelGroupsFromParameters(ChannelGroupParameters Parameters, CancellationToken cancellationToken);
        Task<(int? ChannelGroupId, IEnumerable<VideoStreamDto> VideoStreams)> DeleteChannelGroup(ChannelGroup ChannelGroup);

        //Task AddOrUpdateChannelGroupVideoStreamCounts(List<ChannelGroupStreamCount> channelGroupStreamCounts);
        //ChannelGroupStreamCount? GetChannelGroupVideoStreamCount(int id);
        //IEnumerable<ChannelGroupStreamCount> GetChannelGroupVideoStreamCounts();
        //Task AddOrUpdateChannelGroupVideoStreamCount(ChannelGroupStreamCount response, bool ignoreSave = false);
        Task<List<ChannelGroup>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames);

        IQueryable<string> GetAllChannelGroupNames();

        Task<List<ChannelGroupDto>> GetChannelGroupsFromVideoStreamIds(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken);
        Task<ChannelGroupDto?> GetChannelGroupFromVideoStreamId(string VideoStreamId, CancellationToken cancellationToken);
        Task<int?> GetChannelGroupIdFromVideoStream(string channelGroupName, CancellationToken cancellationToken);
        IQueryable<ChannelGroup> GetAllChannelGroups();
        Task<string?> GetChannelGroupNameFromVideoStream(string videoStreamId, CancellationToken cancellationToken);
        Task<ChannelGroup?> GetChannelGroupByNameAsync(string name);

        Task<ChannelGroupDto?> GetChannelGroupAsync(int Id, CancellationToken cancellationToken = default);

        Task<ChannelGroup?> GetChannelGroupById(int Id);

        Task<PagedResponse<ChannelGroupDto>> GetChannelGroupsAsync(ChannelGroupParameters channelGroupParameters);

        void CreateChannelGroup(ChannelGroup ChannelGroup);

        void UpdateChannelGroup(ChannelGroup ChannelGroup);
        //Task<List<string>> GetChannelNamesFromVideoStream(VideoStreamDto videoStreamDto, CancellationToken cancellationToken);
        //Task<(ChannelGroupDto? channelGroup, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroupIds)> UpdateChannelGroup(UpdateChannelGroupRequest request, string url, CancellationToken cancellationToken);
    }
}