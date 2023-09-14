﻿using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructureEF.Repositories;

/// <summary>
/// Repository to manage EPGFile entities in the database.
/// </summary>
public class EPGFileRepository(ILogger<EPGFileRepository> logger, RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper) : RepositoryBase<EPGFile>(repositoryContext, logger), IEPGFileRepository
{

    /// <summary>
    /// Creates a new EPGFile in the database.
    /// </summary>
    public void CreateEPGFile(EPGFile EPGFile)
    {
        if (EPGFile == null)
        {
            throw new ArgumentNullException(nameof(EPGFile));
        }

        Create(EPGFile);
        logger.LogInformation($"EPGFile with ID {EPGFile.Id} was created.");
    }

    /// <summary>
    /// Deletes an EPGFile from the database.
    /// </summary>
    public async Task<EPGFileDto?> DeleteEPGFile(int EPGFileId)
    {
        if (EPGFileId <= 0)
        {
            throw new ArgumentNullException(nameof(EPGFileId));
        }

        EPGFile? epgFile = await FindByCondition(a => a.Id == EPGFileId).FirstOrDefaultAsync().ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }

        Delete(epgFile);
        logger.LogInformation($"EPGFile with Name {epgFile.Name} was deleted.");
        return mapper.Map<EPGFileDto>(epgFile);
    }

    /// <summary>
    /// Retrieves all EPGFiles from the database.
    /// </summary>
    public async Task<List<EPGFileDto>> GetEPGFiles()
    {
        return await FindAll().ProjectTo<EPGFileDto>(mapper.ConfigurationProvider)
                              .ToListAsync()
                              .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific EPGFile by its ID.
    /// </summary>
    public async Task<EPGFile?> GetEPGFileById(int Id)
    {
        return await FindByCondition(c => c.Id == Id)
                           .AsNoTracking()
                           .FirstOrDefaultAsync()
                           .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific EPGFile by its source.
    /// </summary>
    public async Task<EPGFile?> GetEPGFileBySource(string Source)
    {
        return await FindByCondition(c => c.Source == Source)
                          .AsNoTracking()
                          .FirstOrDefaultAsync()
                          .ConfigureAwait(false);
    }

    public PagedResponse<EPGFileDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<EPGFileDto>(Count());
    }
    public async Task<List<EPGFileDto>> GetEPGFilesNeedUpdating()
    {
        List<EPGFileDto> ret = new();
        List<EPGFileDto> epgFilesToUpdated = await FindByCondition(a => a.AutoUpdate && !string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < DateTime.Now).ProjectTo<EPGFileDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);
        ret.AddRange(epgFilesToUpdated);
        foreach (EPGFile? epgFile in FindByCondition(a => string.IsNullOrEmpty(a.Url)))
        {
            if (epgFile.LastWrite() >= epgFile.LastUpdated)
            {
                ret.Add(mapper.Map<EPGFileDto>(epgFile));
            }
        }
        return ret;
    }

    /// <summary>
    /// Retrieves paged EPGFiles based on specific parameters.
    /// </summary>
    public async Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(EPGFileParameters Parameters)
    {
        if (Parameters == null)
        {
            throw new ArgumentNullException(nameof(Parameters));
        }

        try
        {
            IQueryable<EPGFile> query = GetIQueryableForEntity(Parameters);
            return await query.GetPagedResponseAsync<EPGFile, EPGFileDto>(Parameters.PageNumber, Parameters.PageSize, mapper)
                              .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching and enriching the EPGFiles.");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing EPGFile in the database.
    /// </summary>
    public void UpdateEPGFile(EPGFile EPGFile)
    {
        if (EPGFile == null)
        {
            throw new ArgumentNullException(nameof(EPGFile));
        }

        Update(EPGFile);
        logger.LogInformation($"EPGFile with ID {EPGFile.Id} was updated.");
    }
}
