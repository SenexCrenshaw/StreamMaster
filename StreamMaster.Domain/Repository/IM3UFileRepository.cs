using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;
public interface IM3UFileRepository : IRepositoryBase<M3UFile>
{
    Task<M3UFile?> GetM3UFile(int Id);
    PagedResponse<M3UFileDto> CreateEmptyPagedResponse();

    /// <summary>
    /// Gets a list of M3U file names asynchronously.
    /// </summary>
    Task<List<string>> GetM3UFileNames();

    /// <summary>
    /// Gets a list of all M3U files as M3UFileDto objects asynchronously.
    /// </summary>
    Task<List<M3UFileDto>> GetM3UFiles();

    /// <summary>
    /// Gets a paged list of M3U files based on parameters.
    /// </summary>
    /// <param name="m3uFileParameters">Parameters for paging and filtering.</param>
    Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(QueryStringParameters m3uFileParameters);

    /// <summary>
    /// Gets an M3U file by its source asynchronously.
    /// </summary>
    /// <param name="source">The source of the M3U file to retrieve.</param>
    /// <returns>An M3UFile object if found, or null if not found.</returns>
    Task<M3UFile?> GetM3UFileBySource(string Source);

    Task<List<M3UFileDto>> GetM3UFilesNeedUpdating();

    /// <summary>
    /// Gets the maximum stream count associated with M3U files.
    /// </summary>
    /// <returns>The maximum stream count.</returns>
    Task<int> GetM3UMaxStreamCount();

    /// <summary>
    /// Creates a new M3U file.
    /// </summary>
    /// <param name="m3uFile">The M3U file to create.</param>
    void CreateM3UFile(M3UFile m3uFile);
    /// <summary>
    /// Updates an existing M3U file.
    /// </summary>
    /// <param name="m3uFile">The M3U file to update.</param>
    void UpdateM3UFile(M3UFile m3uFile);

    /// <summary>
    /// Deletes an existing M3U file.
    /// </summary>
    /// <param name="m3uFile">The M3U file to delete.</param>
    Task<M3UFileDto?> DeleteM3UFile(int M3UFileId);

    Task<M3UFileDto?> ChangeM3UFileName(int M3UFileId, string newName);
    Task<M3UFile?> ProcessM3UFile(int M3UFileId, bool ForceRun = false);
}
