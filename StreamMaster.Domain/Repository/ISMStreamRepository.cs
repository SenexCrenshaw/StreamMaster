using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;

public interface ISMStreamRepository
{
    IQueryable<SMStream> GetQuery(bool tracking = false);
    List<SMStreamDto> GetSMStreams();
    PagedResponse<SMStreamDto> CreateEmptyPagedResponse();
    Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters parameters, CancellationToken cancellationToken);
}