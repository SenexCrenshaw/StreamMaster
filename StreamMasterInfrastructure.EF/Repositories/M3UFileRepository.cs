using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructureEF.Repositories;

/// <summary>
/// Provides methods for performing CRUD operations on M3UFile entities.
/// </summary>
public class M3UFileRepository(ILogger<M3UFileRepository> logger, RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper) : RepositoryBase<M3UFile>(repositoryContext, logger), IM3UFileRepository
{

    public PagedResponse<M3UFileDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<M3UFileDto>(Count());
    }
    /// <inheritdoc/>
    public void CreateM3UFile(M3UFile m3uFile)
    {
        if (m3uFile == null)
        {
            logger.LogError("Attempted to create a null M3UFile.");
            throw new ArgumentNullException(nameof(m3uFile));
        }

        Create(m3uFile);
        logger.LogInformation($"Created M3UFile with ID: {m3uFile.Id}.");
    }


    /// <inheritdoc/>
    public async Task<M3UFileDto?> DeleteM3UFile(int M3UFileId)
    {
        if (M3UFileId <= 0)
        {
            throw new ArgumentNullException(nameof(M3UFileId));
        }

        M3UFile? m3uFile = await FindByCondition(a => a.Id == M3UFileId).FirstOrDefaultAsync().ConfigureAwait(false);
        if (m3uFile == null)
        {
            return null;
        }

        Delete(m3uFile);
        logger.LogInformation($"M3UFile with Name {m3uFile.Name} was deleted.");
        return mapper.Map<M3UFileDto>(m3uFile);
    }

    /// <inheritdoc/>
    public async Task<List<M3UFileDto>> GetM3UFiles()
    {
        return await FindAll()
                     .ProjectTo<M3UFileDto>(mapper.ConfigurationProvider)
                     .ToListAsync()
                     .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<M3UFile?> GetM3UFileById(int Id)
    {
        M3UFile? m3uFile = await FindByCondition(c => c.Id == Id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

        return m3uFile;
    }

    /// <inheritdoc/>
    public async Task<M3UFile?> GetM3UFileBySource(string Source)
    {
        M3UFile? m3uFile = await FindByCondition(c => c.Source == Source)
                            .AsNoTracking()
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

        return m3uFile;
    }

    //public async Task<M3UFileDto?> GetM3UFileById(int Id)
    //{
    //    M3UFile? m3uFile = await FindByCondition(c => c.Id == Id)
    //                        .AsNoTracking()
    //                        .FirstOrDefaultAsync()
    //                        .ConfigureAwait(false);

    //    return m3uFile != null ? mapper.Map<M3UFileDto>(m3uFile) : null;
    //}

    /// <inheritdoc/>
    public async Task<int> GetM3UMaxStreamCount()
    {
        return await FindAll().SumAsync(a => a.MaxStreamCount).ConfigureAwait(false);
    }




    /// <inheritdoc/>
    public async Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(M3UFileParameters parameters)
    {
        IQueryable<M3UFile> query = GetIQueryableForEntity(parameters);
        return await query.GetPagedResponseAsync<M3UFile, M3UFileDto>(parameters.PageNumber, parameters.PageSize, mapper)
                          .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void UpdateM3UFile(M3UFile m3uFile)
    {
        if (m3uFile == null)
        {
            logger.LogError("Attempted to update a null M3UFile.");
            throw new ArgumentNullException(nameof(m3uFile));
        }

        Update(m3uFile);
        logger.LogInformation($"Updated M3UFile with ID: {m3uFile.Id}.");
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetM3UFileNames()
    {
        return await FindAll()
                     .OrderBy(a => a.Name)
                     .Select(a => a.Name)
                     .ToListAsync()
                     .ConfigureAwait(false);
    }

    public async Task<List<M3UFileDto>> GetM3UFilesNeedUpdating()
    {
        List<M3UFileDto> ret = new();
        List<M3UFileDto> m3uFilesToUpdated = await FindByCondition(a => a.AutoUpdate && !string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < DateTime.Now).ProjectTo<M3UFileDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);
        ret.AddRange(m3uFilesToUpdated);
        foreach (M3UFile? m3uFile in FindByCondition(a => string.IsNullOrEmpty(a.Url)))
        {
            if (m3uFile.LastWrite() >= m3uFile.LastUpdated)
            {
                ret.Add(mapper.Map<M3UFileDto>(m3uFile));
            }
        }
        return ret;
    }


    public async Task<M3UFileDto?> ChangeM3UFileName(int M3UFileId, string newName)
    {
        M3UFile? m3UFile = await FindByCondition(a => a.Id == M3UFileId).FirstOrDefaultAsync().ConfigureAwait(false);
        if (m3UFile == null)
        {
            return null;
        }
        m3UFile.Name = newName;
        Update(m3UFile);
        await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
        return mapper.Map<M3UFileDto>(m3UFile);
    }

    public IQueryable<M3UFile> GetM3UFileQuery()
    {
        return FindAll();
    }
}
