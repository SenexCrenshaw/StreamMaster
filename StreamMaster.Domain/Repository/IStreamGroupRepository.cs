using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

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

        Task<StreamGroupDto?> UpdateStreamGroup(int StreamGroupId, string? NewName, bool? IgnoreExistingChannelNumbers, int? StartingChannelNumbers);
        Task<int?> DeleteStreamGroup(int streamGroupId);

        //IQueryable<StreamGroup> GetStreamGroupQuery();
        Task<IdIntResultWithResponse> AutoSetSMChannelNumbers(int streamGroupId, int startingNumber, bool overWriteExisting, QueryStringParameters Parameters);

    }
}