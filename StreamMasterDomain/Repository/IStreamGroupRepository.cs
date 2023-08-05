using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IStreamGroupRepository
    {
        Task<List<StreamGroupDto>> GetStreamGroupDtos(string Url, CancellationToken cancellationToken = default);
        IQueryable<StreamGroup> GetAllStreamGroupsWithChannelGroups();
        Task<StreamGroupDto?> GetStreamGroupDto(int id, string Url, CancellationToken cancellationToken = default);
        Task<StreamGroupDto?> UpdateStreamGroupAsync(UpdateStreamGroupRequest request, string Url, CancellationToken cancellationToken);
        Task<StreamGroupDto?> GetStreamGroupDtoByStreamGroupNumber(int streamGroupNumber, string Url, CancellationToken cancellationToken = default);
        Task<bool> AddChannelGroupToStreamGroupAsync(int streamGroupId, int channelGroupId, CancellationToken cancellationToken);
        IQueryable<StreamGroup> GetAllStreamGroups();
        Task<StreamGroup?> GetStreamGroupByIdAsync(int id);
        Task<PagedList<StreamGroup>> GetStreamGroupsAsync(StreamGroupParameters StreamGroupParameters);
        void CreateStreamGroup(StreamGroup StreamGroup);
        Task<bool> DeleteStreamGroupsync(int streamGroupId, CancellationToken cancellationToken);
    }
}