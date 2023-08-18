using AutoMapper;

using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructureEF.Repositories;

public class M3UFileRepository : RepositoryBase<M3UFile>, IM3UFileRepository
{
    private readonly IMapper _mapper;

    public M3UFileRepository(RepositoryContext repositoryContext, IMapper mapper) : base(repositoryContext)
    {
        _mapper = mapper;
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

    public async Task<M3UFile?> GetM3UFileByIdAsync(int Id)
    {
        return await FindByCondition(m3uFile => m3uFile.Id.Equals(Id))
                         .FirstOrDefaultAsync();
    }

    public async Task<int> GetM3UMaxStreamCountAsync()
    {
        List<M3UFile> m3uFiles = await FindAll().ToListAsync();

        return m3uFiles.Sum(a => a.MaxStreamCount);
    }

    public async Task<M3UFile?> GetM3UFileBySourceAsync(string source)
    {
        return await FindByCondition(m3uFile => m3uFile.Source.ToLower().Equals(source.ToLower()))
                          .FirstOrDefaultAsync();
    }

    public async Task<PagedResponse<M3UFileDto>> GetM3UFilesAsync(M3UFileParameters m3uFileParameters)
    {
        return await GetEntitiesAsync<M3UFileDto>(m3uFileParameters, _mapper);

    }

    public async Task<List<string>> GetChannelGroupNamesFromM3UFile(int m3uFileId)
    {
        return await FindByCondition(m3uFile => m3uFile.Id == m3uFileId).Select(a => a.Name).Distinct().ToListAsync();
    }
    public void UpdateM3UFile(M3UFile m3uFile)
    {
        Update(m3uFile);
    }
}