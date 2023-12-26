using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository
{
    public interface IStreamGroupRepository : IRepositoryBase<StreamGroup>
    {
        PagedResponse<StreamGroupDto> CreateEmptyPagedResponse();
        //Task<StreamGroup?> GetStreamGroupWithRelatedEntitiesById(int StreamGroupId, CancellationToken cancellationToken);
        Task<List<StreamGroupDto>> GetStreamGroups(CancellationToken cancellationToken);

        Task<StreamGroupDto?> GetStreamGroupById(int id);

        Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(StreamGroupParameters Parameters);

        void CreateStreamGroup(StreamGroup StreamGroup);

        Task<StreamGroupDto?> UpdateStreamGroup(UpdateStreamGroupRequest request);
        Task<int?> DeleteStreamGroup(int streamGroupId);

        IQueryable<StreamGroup> GetStreamGroupQuery();
    }
}