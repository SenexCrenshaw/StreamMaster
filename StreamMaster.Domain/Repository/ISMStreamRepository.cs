using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;

public interface ISMStreamRepository
{
    IQueryable<SMStream> GetQuery(bool tracking = false);
    List<SMStreamDto> GetSMStreams();
    PagedResponse<SMStreamDto> CreateEmptyPagedResponse();
    Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters parameters, CancellationToken cancellationToken);
    Task<IEnumerable<string>> DeleteAllSMStreamsFromParameters(SMStreamParameters parameters, CancellationToken cancellationToken);
    Task<SMStreamDto?> DeleteSMStreamById(string id, CancellationToken cancellationToken);
    Task<SMStreamDto?> ToggleSMStreamVisibleById(string id, CancellationToken cancellationToken);
    SMStreamDto? GetSMStream(string streamId);
}