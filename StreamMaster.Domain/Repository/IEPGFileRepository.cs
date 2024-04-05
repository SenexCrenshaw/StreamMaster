using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;

public interface IEPGFileRepository : IRepositoryBase<EPGFile>
{
    Task<int> GetNextAvailableEPGNumberAsync(CancellationToken cancellationToken);
    Task<List<EPGFileDto>> GetEPGFilesNeedUpdating();
    Task<List<EPGFileDto>> GetEPGFiles();

    Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(QueryStringParameters Parameters);

    Task<EPGFile?> GetEPGFileById(int EPGNumber);

    Task<EPGFile?> GetEPGFileByNumber(int Id);

    Task<EPGFile?> GetEPGFileBySource(string Source);

    void CreateEPGFile(EPGFile EPGFile);
    PagedResponse<EPGFileDto> CreateEmptyPagedResponse();
    void UpdateEPGFile(EPGFile EPGFile);

    Task<EPGFileDto?> DeleteEPGFile(int EPGFileId);
    Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int id, CancellationToken cancellationToken);
    List<EPGColorDto> GetEPGColors();
}