using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository;

public interface IEPGFileRepository : IRepositoryBase<EPGFile>
{
    Task<List<EPGFileDto>> GetEPGFilesNeedUpdating();
    Task<List<EPGFileDto>> GetEPGFiles();

    Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(EPGFileParameters Parameters);

    Task<EPGFile?> GetEPGFileById(int Id);

    Task<EPGFile?> GetEPGFileBySource(string Source);

    void CreateEPGFile(EPGFile EPGFile);
    PagedResponse<EPGFileDto> CreateEmptyPagedResponse();
    void UpdateEPGFile(EPGFile EPGFile);

    Task<EPGFileDto?> DeleteEPGFile(int EPGFileId);
}