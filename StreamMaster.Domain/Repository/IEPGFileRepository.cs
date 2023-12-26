using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;

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
    Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int id, CancellationToken cancellationToken);
    List<EPGColorDto> GetEPGColors();
}