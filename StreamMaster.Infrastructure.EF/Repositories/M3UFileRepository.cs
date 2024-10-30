using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;

namespace StreamMaster.Infrastructure.EF.Repositories;

/// <summary>
/// Provides methods for performing CRUD operations on M3UFile entities.
/// </summary>
public class M3UFileRepository(ILogger<M3UFileRepository> intLogger, IRepositoryContext repositoryContext, IMapper mapper)
    : RepositoryBase<M3UFile>(repositoryContext, intLogger), IM3UFileRepository
{
    public PagedResponse<M3UFileDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<M3UFileDto>(Count());
    }
    public void CreateM3UFile(M3UFile m3uFile)
    {
        if (m3uFile == null)
        {
            logger.LogError("Attempted to create a null M3UFile.");
            throw new ArgumentNullException(nameof(m3uFile));
        }
        Create(m3uFile);

    }

    /// <inheritdoc/>
    public async Task<M3UFileDto?> DeleteM3UFile(int M3UFileId)
    {
        if (M3UFileId <= 0)
        {
            throw new ArgumentNullException(nameof(M3UFileId));
        }

        M3UFile? m3uFile = await FirstOrDefaultAsync(a => a.Id == M3UFileId).ConfigureAwait(false);
        if (m3uFile == null)
        {
            return null;
        }

        Delete(m3uFile);
        logger.LogInformation("M3UFile with Name {m3uFile.Name} was deleted.", m3uFile.Name);
        return mapper.Map<M3UFileDto>(m3uFile);
    }

    /// <inheritdoc/>
    public async Task<List<M3UFileDto>> GetM3UFiles()
    {
        return await GetQuery().ProjectTo<M3UFileDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);
    }

    public async Task<M3UFile?> GetM3UFileAsync(int Id)
    {
        return await FirstOrDefaultAsync(c => c.Id == Id, false).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously retrieves an M3UFile by its source name, accounting for possible .gz or .zip extensions in both the input and the database.
    /// </summary>
    /// <param name="source">The source name of the M3U file, which might include .gz or .zip extensions.</param>
    /// <returns>The M3UFile if found, or null if no match is found.</returns>
    public async Task<M3UFile?> GetM3UFileBySourceAsync(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        // Normalize source by removing .gz or .zip extensions if present
        string normalizedSource = source.EndsWith(".gz", StringComparison.OrdinalIgnoreCase) || source.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
            ? Path.GetFileNameWithoutExtension(source)
            : source;

        // Define possible variations of the source
        List<string> possibleSources =
        [
        normalizedSource,             // The base source (without extensions)
        $"{normalizedSource}.gz",     // The source with .gz extension
        $"{normalizedSource}.zip",    // The source with .zip extension
        $"{normalizedSource}.m3u",    // The base file as .m3u
        $"{normalizedSource}.m3u.gz", // The .m3u file with .gz extension
        $"{normalizedSource}.m3u.zip" // The .m3u file with .zip extension
    ];

        // Query for a match with any of the possible source variations
        M3UFile? m3uFile = await FirstOrDefaultAsync(c => possibleSources.Contains(c.Source))
            .ConfigureAwait(false);

        return m3uFile;
    }

    /// <inheritdoc/>
    public async Task<int> GetM3UMaxStreamCount()
    {
        return await GetQuery().SumAsync(a => a.MaxStreamCount).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(QueryStringParameters parameters)
    {
        IQueryable<M3UFile> query = GetQuery(parameters);
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
        m3uFile.WriteJSON();

        //logger.LogInformation("Updated M3UFile with ID: {m3uFile.Id}.", m3uFile.Id);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetM3UFileNames()
    {
        return await GetQuery()
                     .OrderBy(a => a.Name)
                     .Select(a => a.Name)
                     .ToListAsync()
                     .ConfigureAwait(false);
    }

    public async Task<List<M3UFileDto>> GetM3UFilesNeedUpdatingAsync()
    {
        // Initialize the result list
        List<M3UFileDto> ret = [];

        // Fetch M3U files that need updating based on AutoUpdate, Url, and HoursToUpdate criteria
        List<M3UFileDto> m3uFilesToUpdate = await GetQuery(a =>
            a.AutoUpdate &&
            (
                (!string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < SMDT.UtcNow) ||
                (string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastUpdated.AddHours(a.HoursToUpdate) < SMDT.UtcNow)
            ))
            .ProjectTo<M3UFileDto>(mapper.ConfigurationProvider)
            .ToListAsync()
            .ConfigureAwait(false);

        // Add files that met the above conditions to the result list
        ret.AddRange(m3uFilesToUpdate);

        // Fetch files without a URL and filter based on LastWrite vs. LastUpdated
        List<M3UFile> filesWithoutUrl = await GetQuery(a => string.IsNullOrEmpty(a.Url))
            .ToListAsync()
            .ConfigureAwait(false);

        // Add files with recent LastWrite activity to the result list
        ret.AddRange(
            filesWithoutUrl
                .Where(m3uFile => m3uFile.LastWrite() >= m3uFile.LastUpdated)
                .Select(mapper.Map<M3UFileDto>)
        );

        return ret;
    }


    public IQueryable<M3UFile> GetM3UFileQuery()
    {
        return GetQuery();
    }
    //private static bool ProcessExistingStream(SMStream stream, SMStream existingStream, M3UFile m3uFile, int index)
    //{
    //    bool changed = false;

    //    if (existingStream.M3UFileId != m3uFile.Id)
    //    {
    //        changed = true;
    //        existingStream.M3UFileId = m3uFile.Id;
    //    }

    //    if (existingStream.ClientUserAgent != stream.ClientUserAgent)
    //    {
    //        changed = true;
    //        existingStream.ClientUserAgent = stream.ClientUserAgent;
    //    }

    //    if (string.IsNullOrEmpty(existingStream.M3UFileName) || existingStream.M3UFileName != m3uFile.Name)
    //    {
    //        changed = true;
    //        existingStream.M3UFileName = m3uFile.Name;
    //    }

    //    if (m3uFile.AutoSetChannelNumbers)
    //    {
    //        stream.ChannelNumber = index + m3uFile.StartingChannelNumber;
    //    }

    //    if (existingStream.ChannelNumber != stream.ChannelNumber)
    //    {
    //        changed = true;
    //        existingStream.ChannelNumber = stream.ChannelNumber;
    //    }

    //    if (existingStream.Group != stream.Group)
    //    {
    //        changed = true;
    //        existingStream.Group = stream.Group;
    //    }

    //    if (existingStream.EPGID != stream.EPGID)
    //    {
    //        changed = true;
    //        existingStream.EPGID = stream.EPGID;
    //    }

    //    if (existingStream.Logo != stream.Logo)
    //    {
    //        changed = true;

    //        existingStream.Logo = stream.Logo;
    //    }

    //    if (existingStream.Url != stream.Url)
    //    {
    //        changed = true;

    //        existingStream.Url = stream.Url;
    //    }

    //    if (existingStream.Name != stream.Name)
    //    {
    //        changed = true;

    //        existingStream.Name = stream.Name;
    //    }

    //    if (existingStream.FilePosition != index)
    //    {
    //        changed = true;

    //        existingStream.FilePosition = index;
    //    }

    //    return changed;
    //}

}
