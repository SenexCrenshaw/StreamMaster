using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Helpers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.Models;
using StreamMaster.SchedulesDirect.Domain.XmltvXml;

namespace StreamMaster.Infrastructure.EF.Repositories;

/// <summary>
/// Repositorywrapper to manage EPGFile entities in the database.
/// </summary>
public class EPGFileRepository(ILogger<EPGFileRepository> intLogger, IXmltv2Mxf xmltv2Mxf, IJobStatusService jobStatusService, IRepositoryContext repositoryContext, ISchedulesDirectDataService schedulesDirectDataService, IMapper mapper)
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
    public async Task<List<EPGFileDto>> GetEPGFiles()
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
    public async Task<EPGFile?> GetEPGFileBySource(string Source)
    {
        return await FirstOrDefaultAsync(c => c.Source == Source).ConfigureAwait(false);
    }

    public PagedResponse<EPGFileDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<EPGFileDto>(Count());
    }
    public async Task<List<EPGFileDto>> GetEPGFilesNeedUpdating()
    {
        List<EPGFileDto> ret = [];
        List<EPGFileDto> epgFilesToUpdated = await GetQuery(a => a.AutoUpdate && !string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < DateTime.Now).ProjectTo<EPGFileDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);
        ret.AddRange(epgFilesToUpdated);
        foreach (EPGFile? epgFile in GetQuery(a => string.IsNullOrEmpty(a.Url)))
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
        logger.LogInformation("EPGFile with number {EPGFile.EPGNumber} was updated.", EPGFile.EPGNumber);
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

            XMLTV? tv = Xmltv2Mxf.ConvertToMxf(Path.Combine(FileDefinitions.EPG.DirectoryLocation, epgFile.Source), epgFile.EPGNumber);

            if (tv != null)
            {
                epgFile.ChannelCount = (tv.Channels?.Count) ?? 0;
                epgFile.ProgrammeCount = (tv.Programs?.Count) ?? 0;
            }

            epgFile.LastUpdated = SMDT.UtcNow;
            UpdateEPGFile(epgFile);
            await SaveChangesAsync();
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
