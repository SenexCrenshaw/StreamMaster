using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

using System.Linq.Expressions;

namespace StreamMaster.Domain.Repository;

public interface ISMStreamRepository : IRepositoryBase<SMStream>
{
    IQueryable<SMStream> GetQuery(Expression<Func<SMStream, bool>> expression, bool tracking = false);
    IQueryable<SMStream> GetQuery(bool tracking = false);
    List<SMStreamDto> GetSMStreams();
    PagedResponse<SMStreamDto> CreateEmptyPagedResponse();
    Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(QueryStringParameters parameters, CancellationToken cancellationToken);
    Task<IEnumerable<string>> DeleteAllSMStreamsFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
    Task<SMStreamDto?> DeleteSMStreamById(string id, CancellationToken cancellationToken);
    Task<SMStreamDto?> ToggleSMStreamVisibleById(string id, CancellationToken cancellationToken);
    SMStreamDto? GetSMStream(string streamId);
    Task DeleteSMStreamsByM3UFiledId(int id, CancellationToken cancellationToken);
    Task<List<FieldData>> ToggleSMStreamsVisibleById(List<string> ids, CancellationToken cancellationToken);
    Task<List<FieldData>> ToggleSMStreamVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken);
}