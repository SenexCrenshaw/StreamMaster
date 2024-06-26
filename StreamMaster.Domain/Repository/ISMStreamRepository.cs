using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

using System.Linq.Expressions;

namespace StreamMaster.Domain.Repository;

public interface ISMStreamRepository : IRepositoryBase<SMStream>
{
    Task ChangeGroupName(string oldGroupName, string newGroupName);

    PagedResponse<SMStreamDto> CreateEmptyPagedResponse();

    Task<IEnumerable<string>> DeleteAllSMStreamsFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken);

    Task<SMStreamDto?> DeleteSMStreamById(string id, CancellationToken cancellationToken);

    Task DeleteSMStreamsByM3UFiledId(int id, CancellationToken cancellationToken);

    Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(QueryStringParameters parameters, CancellationToken cancellationToken);

    IQueryable<SMStream> GetQuery(Expression<Func<SMStream, bool>> expression, bool tracking = false);

    IQueryable<SMStream> GetQuery(bool tracking = false);

    SMStreamDto? GetSMStream(string streamId);

    List<SMStreamDto> GetSMStreams();

    Task<List<FieldData>> SetSMStreamsVisibleById(List<string> ids, bool isHidden, CancellationToken cancellationToken);

    Task<List<FieldData>> ToggleSMStreamsVisibleById(List<string> ids, CancellationToken cancellationToken);

    Task<SMStreamDto?> ToggleSMStreamVisibleById(string id, CancellationToken cancellationToken);

    Task<List<FieldData>> ToggleSMStreamVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
}