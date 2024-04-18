using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Domain.Repository
{
    public interface IStreamGroupRepository : IRepositoryBase<StreamGroup>
    {
        PagedResponse<StreamGroupDto> CreateEmptyPagedResponse();
        //Task<StreamGroup?> GetStreamGroupWithRelatedEntitiesById(int StreamGroupId, CancellationToken cancellationToken);
        Task<List<StreamGroupDto>> GetStreamGroups(CancellationToken cancellationToken);

        StreamGroup? GetStreamGroup(int id);
        Task<StreamGroupDto?> GetStreamGroupById(int id);

        Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters);

        void CreateStreamGroup(StreamGroup StreamGroup);

        Task<StreamGroupDto?> UpdateStreamGroup(UpdateStreamGroupRequest request);
        Task<int?> DeleteStreamGroup(int streamGroupId);

        IQueryable<StreamGroup> GetStreamGroupQuery();
    }
}