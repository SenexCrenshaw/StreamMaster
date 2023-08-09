using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructureEF;

public class EPGFileRepository : RepositoryBase<EPGFile>, IEPGFileRepository
{
    private readonly ISortHelper<EPGFile> _EPGFileSortHelper;

    public EPGFileRepository(RepositoryContext repositoryContext, ISortHelper<EPGFile> EPGFileSortHelper) : base(repositoryContext)
    {
        _EPGFileSortHelper = EPGFileSortHelper;
    }

    public void CreateEPGFile(EPGFile EPGFile)
    {
        Create(EPGFile);
    }

    public void DeleteEPGFile(EPGFile EPGFile)
    {
        Delete(EPGFile);
    }

    public async Task<IEnumerable<EPGFile>> GetAllEPGFilesAsync()
    {
        return await FindAll()
                        .OrderBy(p => p.Id)
                        .ToListAsync();
    }

    public async Task<EPGFile> GetEPGFileByIdAsync(int Id)
    {
        return await FindByCondition(EPGFile => EPGFile.Id.Equals(Id))
                         .FirstOrDefaultAsync();
    }

    public async Task<EPGFile> GetEPGFileBySourceAsync(string source)
    {
        return await FindByCondition(EPGFile => EPGFile.Source.ToLower().Equals(source.ToLower()))
                          .FirstOrDefaultAsync();
    }

    public async Task<IPagedList<EPGFile>> GetEPGFilesAsync(EPGFileParameters EPGFileParameters)
    {
        IQueryable<EPGFile> EPGFiles = FindAll();

        IQueryable<EPGFile> sorderEPGFiles = _EPGFileSortHelper.ApplySort(EPGFiles, EPGFileParameters.OrderBy);

        return await sorderEPGFiles.ToPagedListAsync(EPGFileParameters.PageNumber, EPGFileParameters.PageSize).ConfigureAwait(false);
    }

    public void UpdateEPGFile(EPGFile EPGFile)
    {
        Update(EPGFile);
    }
}