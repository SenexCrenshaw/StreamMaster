using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IChannelGroupRepository
    {
        IQueryable<string> GetAllChannelGroupNames();
        IQueryable<ChannelGroup> GetAllChannelGroups();

        Task<ChannelGroup?> GetChannelGroupByNameAsync(string name);

        Task<ChannelGroupDto?> GetChannelGroupAsync(int Id, CancellationToken cancellationToken = default);

        Task<PagedResponse<ChannelGroupDto>> GetChannelGroupsAsync(ChannelGroupParameters channelGroupParameters);

        void CreateChannelGroup(ChannelGroup ChannelGroup);

        void DeleteChannelGroup(ChannelGroup ChannelGroup);

        void UpdateChannelGroup(ChannelGroup ChannelGroup);
        Task<List<string>> GetChannelNamesFromVideoStream(VideoStreamDto videoStreamDto, CancellationToken cancellationToken);
        Task<(ChannelGroupDto? channelGroup, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroupIds)> UpdateChannelGroup(UpdateChannelGroupRequest request, string url, CancellationToken cancellationToken);
    }
}