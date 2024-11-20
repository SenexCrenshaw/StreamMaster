using System.Diagnostics;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Helpers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.Models;
using StreamMaster.Streams.Domain.Interfaces;

namespace StreamMaster.Infrastructure.EF.Repositories;

/// <summary>
/// Repositorywrapper to manage EPGFile entities in the database.
/// </summary>
public class EPGFileRepository(ILogger<EPGFileRepository> intLogger, IFileUtilService fileUtilService, ICacheManager cacheManager, IXmltv2Mxf xmltv2Mxf, IJobStatusService jobStatusService, IRepositoryContext repositoryContext, ISchedulesDirectDataService schedulesDirectDataService, IMapper mapper)
    : RepositoryBase<EPGFile>(repositoryContext, intLogger), IEPGFileRepository
{
    public IXmltv2Mxf Xmltv2Mxf { get; } = xmltv2Mxf;

    public async Task<int> GetNextAvailableEPGNumberAsync(CancellationToken cancellationToken)
    {
        List<int> epgNumbers = await GetQuery()
                                        .Select(x => x.EPGNumber)
                                        .OrderBy(x => x)
                                        .ToListAsync(cancellationToken)
                                        .ConfigureAwait(false);

        int nextAvailableNumber = 1;
        foreach (int num in epgNumbers)
        {
            if (num != nextAvailableNumber)
            {
                break;
            }
            nextAvailableNumber++;
        }

        return nextAvailableNumber;
    }

    public async Task<EPGFile?> GetEPGFile(int Id)
    {
        return await FirstOrDefaultAsync(c => c.Id == Id, false).ConfigureAwait(false);
    }

    public async Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int Id, CancellationToken cancellationToken)
    {
        if (Id < 0)
        {
            return [];
        }

        EPGFile? epgFile = await FirstOrDefaultAsync(a => a.Id == Id, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (epgFile == null)
        {
            return [];
        }

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.GetEPGData(epgFile.EPGNumber);

        ICollection<MxfService> services = schedulesDirectData.Services.Values;
        List<EPGFilePreviewDto> ret = [];
        foreach (MxfService? service in services)
        {
            if (service is null || string.IsNullOrEmpty(service.Name) || string.IsNullOrEmpty(service.StationId))
            {
                continue;
            }

            string stationId = service.StationId;
            if (EPGHelper.IsValidEPGId(service.StationId))
            {
                (int EPGNumber, stationId) = service.StationId.ExtractEPGNumberAndStationId();
            }

            ret.Add(new EPGFilePreviewDto
            {
                Id = service.Id,
                ChannelName = service.Name,
                ChannelNumber = stationId,
                ChannelLogo = service?.mxfGuideImage?.ImageUrl ?? "",
            });
        }
        return ret;
    }

    /// <summary>
    /// Creates a new EPGFile in the database.
    /// </summary>
    public void CreateEPGFile(EPGFile EPGFile)
    {
        ArgumentNullException.ThrowIfNull(EPGFile);

        Create(EPGFile);
        logger.LogInformation("EPGFile with number {EPGFile.EPGNumber} was created.", EPGFile.EPGNumber);
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

        EPGFile? epgFile = await FirstOrDefaultAsync(a => a.Id == EPGFileId).ConfigureAwait(false);
        if (epgFile == null)
        {
            return null;
        }

        Delete(epgFile);
        logger.LogInformation("EPGFile with Name {epgFile.Name} was deleted.", epgFile.Name);
        return mapper.Map<EPGFileDto>(epgFile);
    }

    /// <summary>
    /// Retrieves all EPGFiles from the database.
    /// </summary>
    public async Task<List<EPGFileDto>> GetEPGFilesAsync()
    {
        return await GetQuery().ProjectTo<EPGFileDto>(mapper.ConfigurationProvider)
                              .ToListAsync()
                              .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific EPGFile by its ID.
    /// </summary>
    public async Task<EPGFile?> GetEPGFileById(int Id)
    {
        return await FirstOrDefaultAsync(c => c.Id == Id).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific EPGFile by its source.
    /// </summary>
    public async Task<EPGFile?> GetEPGFileBySourceAsync(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        // Normalize source by removing .gz or .zip extensions if present
        string normalizedSource = source.EndsWithIgnoreCase(".gz") || source.EndsWithIgnoreCase(".zip")
            ? Path.GetFileNameWithoutExtension(source)
            : source;

        // Define possible variations of the source
        List<string> possibleSources =
        [
        normalizedSource,             // The base source (without extensions)
        $"{normalizedSource}.gz",     // The source with .gz extension
        $"{normalizedSource}.zip",    // The source with .zip extension
        $"{normalizedSource}.xml",    // The base file as .xml
        $"{normalizedSource}.xml.gz", // The .xml file with .gz extension
        $"{normalizedSource}.xml.zip" // The .xml file with .zip extension
    ];

        // Query for a match with any of the possible source variations
        EPGFile? epgFile = await FirstOrDefaultAsync(c => possibleSources.Contains(c.Source)).ConfigureAwait(false);

        return epgFile;
    }

    public PagedResponse<EPGFileDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<EPGFileDto>(Count());
    }

    public async Task<List<EPGFileDto>> GetEPGFilesNeedUpdatingAsync()
    {
        List<EPGFile> filesWithoutUrl = await GetQuery(a => string.IsNullOrEmpty(a.Url))
            .ToListAsync()
            .ConfigureAwait(false);

        HashSet<EPGFileDto> toUpdate = filesWithoutUrl.Where(epgFile => epgFile.LastWrite() >= epgFile.LastUpdated).Select(mapper.Map<EPGFileDto>).ToHashSet();

        if (toUpdate.Count > 0)
        {
            logger.LogInformation("Found {toUpdate.Count} EPG files that need updating based on LastWrite and LastUpdated criteria.", toUpdate.Count);
        }

        List<EPGFileDto> ret = await GetQuery(a =>
            a.AutoUpdate && !string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < SMDT.UtcNow)
            .ProjectTo<EPGFileDto>(mapper.ConfigurationProvider)
            .ToListAsync()
            .ConfigureAwait(false);

        if (ret.Count > 0)
        {
            logger.LogInformation("Found {ret.Count} EPG files that need updating based on LastDownloaded criteria.", ret.Count);
            foreach (EPGFileDto r in ret)
            {
                toUpdate.Add(r);
            }
        }

        ret = await GetQuery(a =>
            a.AutoUpdate && string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastUpdated.AddHours(a.HoursToUpdate) < SMDT.UtcNow)
            .ProjectTo<EPGFileDto>(mapper.ConfigurationProvider)
            .ToListAsync()
            .ConfigureAwait(false);

        if (ret.Count > 0)
        {
            logger.LogInformation("Found {ret.Count} EPG files that need updating based on LastUpdated criteria.", ret.Count);
            foreach (EPGFileDto r in ret)
            {
                toUpdate.Add(r);
            }
        }

        return [.. toUpdate];
    }

    /// <summary>
    /// Retrieves paged EPGFiles based on specific parameters.
    /// </summary>
    public async Task<PagedResponse<EPGFileDto>> GetPagedEPGFiles(QueryStringParameters Parameters)
    {
        ArgumentNullException.ThrowIfNull(Parameters);

        try
        {
            IQueryable<EPGFile> query = GetQuery(Parameters);
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
        ArgumentNullException.ThrowIfNull(EPGFile);

        Update(EPGFile);
        //logger.LogInformation("EPGFile with number {EPGFile.EPGNumber} was updated.", EPGFile.EPGNumber);
    }

    public List<EPGColorDto> GetEPGColors()
    {
        return [.. GetQuery().ProjectTo<EPGColorDto>(mapper.ConfigurationProvider)];
    }

    public async Task<EPGFile?> GetEPGFileByNumber(int EPGNumber)
    {
        return await FirstOrDefaultAsync(a => a.EPGNumber == EPGNumber);
    }

    public async Task<EPGFile?> ProcessEPGFile(int EPGFileId)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManagerProcessEPG(EPGFileId);
        if (jobManager.IsRunning)
        {
            return null;
        }

        jobManager.Start();
        EPGFile? epgFile = null;
        try
        {
            epgFile = await GetEPGFile(EPGFileId);
            if (epgFile == null)
            {
                logger.LogCritical("Could not find EPG file");
                jobManager.SetError();
                return null;
            }
            cacheManager.ClearEPGDataByEPGNumber(epgFile.EPGNumber);
            Stopwatch timer = Stopwatch.StartNew();

            (int channelCount, int programCount) = await fileUtilService.ReadXmlCountsFromFileAsync(epgFile);

            if (channelCount != -1)
            {
                epgFile.ChannelCount = channelCount;
                epgFile.ProgrammeCount = programCount;
            }

            epgFile.LastUpdated = SMDT.UtcNow;
            UpdateEPGFile(epgFile);
            await SaveChangesAsync();
            await fileUtilService.ProcessStationChannelNamesAsync(epgFile);

            jobManager.SetSuccessful();
            return epgFile;
        }
        catch (Exception ex)
        {
            jobManager.SetError();
            logger.LogCritical(ex, "Error while processing M3U file");
            return null;
        }
        finally
        {
            if (epgFile != null)
            {
                epgFile.LastUpdated = SMDT.UtcNow;
                UpdateEPGFile(epgFile);
                await SaveChangesAsync();
            }
        }
    }
}