using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository
{
    public interface IM3UFileRepository : IRepositoryBase<M3UFile, M3UFileDto>
    {
        IQueryable<string> GetM3UFileNames();
        Task<List<string>> GetChannelGroupNamesFromM3UFile(int m3uFileId);
        Task<IEnumerable<M3UFile>> GetAllM3UFilesAsync();

        Task<PagedResponse<M3UFileDto>> GetM3UFilesAsync(M3UFileParameters m3uFileParameters);

        Task<M3UFile> GetM3UFileByIdAsync(int Id);

        Task<M3UFile> GetM3UFileBySourceAsync(string source);

        Task<int> GetM3UMaxStreamCountAsync();

        void CreateM3UFile(M3UFile m3uFile);

        void UpdateM3UFile(M3UFile m3uFile);

        void DeleteM3UFile(M3UFile m3uFile);
    }
}