using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository;

public interface IEPGFileRepository : IRepositoryBase<EPGFile>
{
    IQueryable<EPGFile> GetEPGFileQuery();
    Task<List<EPGFileDto>> GetEPGFilesNeedUpdating();
    Task<List<EPGFileDto>> GetEPGFiles();

    Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(EPGFileParameters Parameters);

    Task<EPGFileDto?> GetEPGFileById(int Id);

    Task<EPGFileDto?> GetEPGFileBySource(string source);

    void CreateEPGFile(EPGFile EPGFile);

    void UpdateEPGFile(EPGFile EPGFile);

    Task<EPGFileDto?> DeleteEPGFile(int EPGFileId);
}