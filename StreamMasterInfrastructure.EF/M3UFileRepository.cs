using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructure.EF;

public class M3UFileRepository : RepositoryBase<M3UFile>, IM3UFileRepository
{
    private ISortHelper<M3UFile> _m3uFileSortHelper;
    public M3UFileRepository(RepositoryContext repositoryContext, ISortHelper<M3UFile> m3uFileSortHelper) : base(repositoryContext)
    {
        _m3uFileSortHelper = m3uFileSortHelper;
    }

    public void CreateM3UFile(M3UFile m3uFile)
    {
        Create(m3uFile);
    }

    public void DeleteM3UFile(M3UFile m3uFile)
    {
        Delete(m3uFile);
    }

    public async Task<IEnumerable<M3UFile>> GetAllM3UFilesAsync()
    {
        return await FindAll()
                        .OrderBy(p => p.Id)
                        .ToListAsync();
    }

    public async Task<M3UFile> GetM3UFileByIdAsync(int Id)
    {
        return await FindByCondition(m3uFile => m3uFile.Id.Equals(Id))
                         .FirstOrDefaultAsync();
    }

    public async Task<int> GetM3UMaxStreamCountAsync()
    {
        var m3uFiles = await FindAll().ToListAsync();

        return m3uFiles.Sum(a => a.MaxStreamCount);

    }

    public async Task<M3UFile> GetM3UFileBySourceAsync(string source)
    {
        return await FindByCondition(m3uFile => m3uFile.Source.ToLower().Equals(source.ToLower()))
                          .FirstOrDefaultAsync();
    }

    public async Task<PagedList<M3UFile>> GetM3UFilesAsync(M3UFileParameters m3uFileParameters)
    {
        var m3uFiles = FindAll();

        var sorderM3UFiles = _m3uFileSortHelper.ApplySort(m3uFiles, m3uFileParameters.OrderBy);

        return await PagedList<M3UFile>.ToPagedList(sorderM3UFiles, m3uFileParameters.PageNumber, m3uFileParameters.PageSize);
    }

    public void UpdateM3UFile(M3UFile m3uFile)
    {
        Update(m3uFile);
    }
}
