using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IStreamGroupRepository : IRepositoryBase<StreamGroup>
    {
        Task AddStreamGroupRequestAsync(AddStreamGroupRequest request, CancellationToken cancellationToken);
        //Task<StreamGroupDto?> Sync(int streamGroupId, List<string>? ChannelGroupNames, List<VideoStreamIsReadOnly>? VideoStreams, CancellationToken cancellationToken = default);
        Task<StreamGroup?> GetStreamGroupWithRelatedEntitiesByIdAsync(int StreamGroupId, CancellationToken cancellationToken);

        Task SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken);
        Task<List<StreamGroupDto>> GetStreamGroupDtos(CancellationToken cancellationToken = default);

        IQueryable<StreamGroup> GetAllStreamGroupsWithChannelGroups();

        Task<StreamGroupDto?> GetStreamGroupDto(int id, CancellationToken cancellationToken = default);

        Task<StreamGroupDto?> UpdateStreamGroupAsync(UpdateStreamGroupRequest request, CancellationToken cancellationToken);

        Task<StreamGroupDto?> GetStreamGroupDtoByStreamGroupNumber(int streamGroupNumber, CancellationToken cancellationToken = default);

        Task<bool> AddChannelGroupToStreamGroupAsync(int streamGroupId, int channelGroupId, CancellationToken cancellationToken);

        IQueryable<StreamGroup> GetAllStreamGroups();

        Task<StreamGroup?> GetStreamGroupByIdAsync(int id);

        PagedResponse<StreamGroupDto> CreateEmptyPagedResponse(StreamGroupParameters Parameters);
        Task<IPagedList<StreamGroup>> GetStreamGroupsAsync(StreamGroupParameters StreamGroupParameters);

        Task<PagedResponse<StreamGroupDto>> GetStreamGroupDtosPagedAsync(StreamGroupParameters StreamGroupParameters);

        void CreateStreamGroup(StreamGroup StreamGroup);

        Task<bool> DeleteStreamGroupsync(int streamGroupId, CancellationToken cancellationToken);
    }
}