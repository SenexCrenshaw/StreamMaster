using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

using System.Threading.Tasks;

namespace StreamMasterDomain.Repository
{
    public interface IStreamGroupRepository : IRepositoryBase<StreamGroup>
    {
        Task AddVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default);
        Task RemoveVideoStreamFromStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default);

        Task<List<VideoStreamDto>> GetStreamGroupVideoStreams(int id, CancellationToken cancellationToken = default);
        Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(int id, CancellationToken cancellationToken = default);
        Task SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken);
        Task<List<StreamGroupDto>> GetStreamGroupDtos(string Url, CancellationToken cancellationToken = default);

        IQueryable<StreamGroup> GetAllStreamGroupsWithChannelGroups();

        Task<StreamGroupDto?> GetStreamGroupDto(int id, string Url, CancellationToken cancellationToken = default);

        Task<StreamGroupDto?> UpdateStreamGroupAsync(UpdateStreamGroupRequest request, string Url, CancellationToken cancellationToken);

        Task<StreamGroupDto?> GetStreamGroupDtoByStreamGroupNumber(int streamGroupNumber, string Url, CancellationToken cancellationToken = default);

        Task<bool> AddChannelGroupToStreamGroupAsync(int streamGroupId, int channelGroupId, CancellationToken cancellationToken);

        IQueryable<StreamGroup> GetAllStreamGroups();

        Task<StreamGroup?> GetStreamGroupByIdAsync(int id);

        Task<IPagedList<StreamGroup>> GetStreamGroupsAsync(StreamGroupParameters StreamGroupParameters);

        Task<PagedResponse<StreamGroupDto>> GetStreamGroupDtosPagedAsync(StreamGroupParameters StreamGroupParameters, string Url);

        void CreateStreamGroup(StreamGroup StreamGroup);

        Task<bool> DeleteStreamGroupsync(int streamGroupId, CancellationToken cancellationToken);
    }
}