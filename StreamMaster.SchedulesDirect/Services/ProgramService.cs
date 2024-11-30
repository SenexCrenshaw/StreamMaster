using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Threading.Channels;

using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Cache;

namespace StreamMaster.SchedulesDirect.Services;
public class ProgramService(
    ILogger<ProgramService> logger,
    IMemoryCache memoryCache,
    ILogger<HybridCacheManager<ProgramService>> cacheLogger,
    ISchedulesDirectAPIService schedulesDirectAPI,
    ISchedulesDirectDataService schedulesDirectDataService) : IProgramService, IDisposable
{
    private readonly HybridCacheManager<ProgramService> hybridCache = new(cacheLogger, memoryCache, useCompression: false, useKeyBasedFiles: true);
    private readonly Channel<string> programChannel = Channel.CreateUnbounded<string>();
    private readonly ConcurrentBag<Programme> programResponses = [];
    private readonly SemaphoreSlim semaphore = new(SchedulesDirect.MaxParallelDownloads);

    public async Task<bool> BuildProgramEntriesAsync(CancellationToken cancellationToken)
    {
        await FillChannelWithProgramsAsync(cancellationToken);

        // Create parallel consumers that fetch and process data in real-time
        List<Task> consumerTasks = [];
        for (int i = 0; i < SchedulesDirect.MaxParallelDownloads; i++)
        {
            consumerTasks.Add(Task.Run(() => FetchAndProcessProgramsAsync(cancellationToken), cancellationToken));
        }

        await Task.WhenAll(consumerTasks).ConfigureAwait(false);

        logger.LogInformation("Finished processing all programs.");
        return true;
    }

    private async Task FetchAndProcessProgramsAsync(CancellationToken cancellationToken)
    {
        await foreach (string programId in programChannel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            try
            {
                // Fetch program data
                List<Programme>? responses = await GetProgramsAsync(new[] { programId }, cancellationToken).ConfigureAwait(false);

                if (responses != null)
                {
                    // Process each response immediately
                    await Task.WhenAll(responses.Select(response =>
                        Task.Run(() => ProcessSingleProgramAsync(response, cancellationToken))));
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Failed to process program ID {ProgramId}: {Error}", programId, ex.Message);
            }
        }
    }


    private async Task FillChannelWithProgramsAsync(CancellationToken cancellationToken)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();

        await Parallel.ForEachAsync(schedulesDirectData.Programs.Values, cancellationToken, async (mxfProgram, ct) =>
        {
            if (!string.IsNullOrEmpty(mxfProgram.MD5))
            {
                string? cachedJson = await hybridCache.GetAsync(mxfProgram.MD5).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(cachedJson))
                {
                    try
                    {
                        Programme? sdProgram = JsonSerializer.Deserialize<Programme>(cachedJson);
                        if (sdProgram != null)
                        {
                            BuildMxfProgram(mxfProgram, sdProgram);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning("Cache deserialization failed for Program ID {ProgramId}: {Error}", mxfProgram.ProgramId, ex.Message);
                    }
                }
            }

            await programChannel.Writer.WriteAsync(mxfProgram.ProgramId, ct).ConfigureAwait(false);
        });


    }

    private async Task<List<Programme>?> GetProgramsAsync(string[] programIds, CancellationToken cancellationToken)
    {
        try
        {
            return await schedulesDirectAPI
                .GetApiResponse<List<Programme>?>(APIMethod.POST, "programs", programIds, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError("API fetch failed for program IDs: {Error}", ex.Message);
            return null;
        }
    }

    private async Task ProcessSingleProgramAsync(Programme sdProgram, CancellationToken cancellationToken)
    {
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        MxfProgram mxfProgram = schedulesDirectData.FindOrCreateProgram(sdProgram.ProgramId);

        BuildMxfProgram(mxfProgram, sdProgram);

        if (!string.IsNullOrEmpty(sdProgram.Md5))
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(sdProgram);
                await hybridCache.SetAsync(sdProgram.Md5, jsonString).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError("Failed to cache program {ProgramId}: {Error}", sdProgram.ProgramId, ex.Message);
            }
        }
        else
        {
            logger.LogInformation("Did not cache program {ProgramId} due to missing MD5 hash.", sdProgram.ProgramId);
        }
    }

    private void BuildMxfProgram(MxfProgram mxfProgram, Programme sdProgram)
    {
        // Populate program properties from the sdProgram
        SetProgramFlags(mxfProgram, sdProgram);
        DetermineTitlesAndDescriptions(mxfProgram, sdProgram);
    }

    private void SetProgramFlags(MxfProgram prg, Programme sd)
    {
        if (sd.Genres is null)
        {
            return;
        }

        string[] types = [sd.EntityType, sd.ShowType];

        prg.IsAction = SDHelpers.TableContains(sd.Genres, "Action") || SDHelpers.TableContains(sd.Genres, "Adventure");
        prg.IsAdultOnly = SDHelpers.TableContains(sd.Genres, "Adults Only");
        prg.IsComedy = SDHelpers.TableContains(sd.Genres, "Comedy");
        prg.IsDocumentary = SDHelpers.TableContains(sd.Genres, "Documentary");
        prg.IsDrama = SDHelpers.TableContains(sd.Genres, "Drama");
        prg.IsEducational = SDHelpers.TableContains(sd.Genres, "Educational");
        prg.IsHorror = SDHelpers.TableContains(sd.Genres, "Horror");
        prg.IsIndy = SDHelpers.TableContains(sd.Genres, "Independent") || SDHelpers.TableContains(sd.Genres, "Indy");
        prg.IsKids = SDHelpers.TableContains(sd.Genres, "Children") || SDHelpers.TableContains(sd.Genres, "Kids");
        prg.IsMusic = SDHelpers.TableContains(sd.Genres, "Music") || SDHelpers.TableContains(types, "Music");
        prg.IsNews = SDHelpers.TableContains(sd.Genres, "News");
        prg.IsReality = SDHelpers.TableContains(sd.Genres, "Reality");
        prg.IsRomance = SDHelpers.TableContains(sd.Genres, "Romance") || SDHelpers.TableContains(sd.Genres, "Romantic");
        prg.IsScienceFiction = SDHelpers.TableContains(sd.Genres, "Science Fiction");
        prg.IsSoap = SDHelpers.TableContains(sd.Genres, "Soap");
        prg.IsThriller = SDHelpers.TableContains(sd.Genres, "Suspense") || SDHelpers.TableContains(sd.Genres, "Thriller");

        prg.IsMiniseries = SDHelpers.TableContains(types, "Miniseries");
        prg.IsMovie = sd.ProgramId.StartsWith("MV") || SDHelpers.TableContains(types, "Movie");
        prg.IsPaidProgramming = SDHelpers.TableContains(types, "Paid Program");
        prg.IsSeries = SDHelpers.TableContains(types, "Series") && !SDHelpers.TableContains(sd.Genres, "Sports talk");
        prg.IsSpecial = SDHelpers.TableContains(types, "Special");
        prg.IsSports = sd.ProgramId.StartsWith("SP") || SDHelpers.TableContains(types, "Event") || SDHelpers.TableContains(sd.Genres, "Sports talk");

        // Add the sports event to SportEvents for tracking
        //if (prg.IsSports)
        //{
        //    sportsImages.SportEvents.Add(prg);
        //}
    }
    // Helper method: Determine the titles and descriptions for a program
    private static void DetermineTitlesAndDescriptions(MxfProgram mxfProgram, Programme sdProgram)
    {
        // Populate titles
        mxfProgram.Title = sdProgram.Titles?[0]?.Title120 ?? string.Empty;
        mxfProgram.EpisodeTitle = sdProgram.EpisodeTitle150;

        // Populate descriptions and language
        if (sdProgram.Descriptions != null)
        {
            // Get the language from the ShortDescription
            mxfProgram.ShortDescription = GetDescriptions(sdProgram.Descriptions.Description100, out string? shortDescriptionLang);

            // Get the language from the Description
            mxfProgram.Description = GetDescriptions(sdProgram.Descriptions.Description1000, out string? descriptionLang);

            // Populate the language: use ShortDescription language first, fallback to Description language if necessary
            if (!string.IsNullOrEmpty(shortDescriptionLang))
            {
                mxfProgram.Language = shortDescriptionLang.ToLower();
            }
            else if (!string.IsNullOrEmpty(descriptionLang))
            {
                mxfProgram.Language = descriptionLang.ToLower();
            }
        }

        mxfProgram.OriginalAirdate = sdProgram.OriginalAirDate.ToString();
    }

    // Helper method: Get descriptions from a program's data
    private static string GetDescriptions(List<ProgramDescription> descriptions, out string language)
    {
        string ret = string.Empty;
        language = string.Empty;

        if (descriptions == null)
        {
            return ret;
        }

        foreach (ProgramDescription description in descriptions)
        {
            if (description.DescriptionLanguage[..2] == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
            {
                language = description.DescriptionLanguage;
                ret = description.Description;
                break;
            }

            if (description.DescriptionLanguage[..2].EqualsIgnoreCase("en") || description.DescriptionLanguage.EqualsIgnoreCase("und"))
            {
                language = description.DescriptionLanguage;
                ret = description.Description;
            }
        }
        return ret;
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            semaphore.Dispose();
            programChannel.Writer.Complete();
        }
    }

    public List<string> GetExpiredKeys()
    {
        return hybridCache.GetExpiredKeysAsync().Result;
    }

    public void RemovedExpiredKeys(List<string>? keysToDelete = null) { }
}
